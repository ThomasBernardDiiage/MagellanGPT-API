using Magellan.Domain;
using Magellan.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.KernelMemory;
using System.Text.Json;
using Magellan.Api.Dto.Down;

namespace Magellan.Api.Controllers;

[Authorize]
[Route("api/tcu/questions")]
public class TcuController : ControllerBase
{
    private readonly MemoryServerless _memoryServerless;
    
    public TcuController(MemoryServerless memoryServerless)
    {
        _memoryServerless = memoryServerless;
    }
    
    [HttpGet]
    public IActionResult GetAllQuestions()
    {
        var questions = GetTcuQuestions();
        return Ok(questions);
    }
    
    [HttpPost("answers")]
    public async Task<IActionResult> GetAnswers([FromForm] IFormFile file)
    {
        // 1) Import document
        await _memoryServerless.ImportDocumentAsync(file.OpenReadStream(), documentId:file.FileName, fileName: file.FileName);
        var imported = await _memoryServerless.IsDocumentReadyAsync(file.FileName);
        if (!imported) throw new DocumentNotImportedException();
        
        // 2) Prepare prompt
        var questions = GetTcuQuestions().OrderBy(x => x.Index);
        
        var prompt = $$"""
                       You will receive a series of questions about the {{file.FileName}} file. Each question will be separated by ###.
                       
                       Example: 
                       ###
                       What is the publication date of this document?
                       ###
                       What are the personal information collected?
                       
                       You must answer each question briefly (1 or 2 sentences). You must answer the questions in order. You must reply in JSON format, example :
                       
                       [
                            "The publication date is 2021-01-01."
                            "The personal information collected are name and email."
                       ]
                       
                       Here is the list of questions:
                       
                       {{string.Join("\n###\n", questions.Select(x => x.Text)) }}
                       """;
        

        var response = await _memoryServerless.AskAsync(prompt);

        await _memoryServerless.DeleteDocumentAsync(file.FileName);
        
        var answers = JsonSerializer.Deserialize<IEnumerable<string>>(response.Result);

        
        return Ok(answers.Select((answer, i) =>
        {
            var question = questions.ElementAt(i);
            return new TcuAnswerDtoDown(question.Index, question.Text, answer);
        }));
    }

    private IEnumerable<Question> GetTcuQuestions()
    {
        var question1 = new Question { Index = 1, Text = "Quelle est la date de publication de ce document ?" };
        var question2 = new Question { Index = 3, Text = "Quelles sont les informations personnelles collectées ?" };
        var question3 = new Question { Index = 4, Text = "Quelle est la durée de conservation des données ?" };
        var question4 = new Question { Index = 5, Text = "Quels sont les droits des utilisateurs ?" };
        var question5 = new Question { Index = 6, Text = "Quelle est la finalité de la collecte des données ?" };
        
        return new List<Question> { question1, question2, question3, question4, question5 };
    }
    
}