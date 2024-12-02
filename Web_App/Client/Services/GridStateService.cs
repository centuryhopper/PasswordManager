
namespace Client.Services;

using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using Client.Models;
using Client.Utils;
using HandyBlazorComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.Models;
using static HandyBlazorComponents.Models.ServiceResponses;

public class GridStateService : IHandyGridState<HandyGridEntity, PasswordAccountDTO>
{
    public GridValidationResponse ValidationChecks(HandyGridEntity item, List<string> columns)
    {
        //var columns = typeof(PasswordAccountDTO).GetProperties().Select(prop => prop.Name).Except(ReadonlyColumns).ToList();
        Dictionary<int, List<string>> errorMessagesDict = new();

        int titleIndex = columns.IndexOf(nameof(item.Object.Title));
        int userNameIndex = columns.IndexOf(nameof(item.Object.Username));
        int passwordIndex = columns.IndexOf(nameof(item.Object.Password));

        // Console.WriteLine($"{titleIndex},{userNameIndex},{passwordIndex}");

        if (string.IsNullOrWhiteSpace(item.Object.Title))
        {
            if (errorMessagesDict.ContainsKey(titleIndex))
            {
                errorMessagesDict[titleIndex].Add($"Please fill out {nameof(item.Object.Title)}");
            }
            else
            {
                errorMessagesDict.Add(titleIndex, [$"Please fill out {nameof(item.Object.Title)}"]);
            }
        }
        if (string.IsNullOrWhiteSpace(item.Object.Username))
        {
            if (errorMessagesDict.ContainsKey(userNameIndex))
            {
                errorMessagesDict[userNameIndex].Add($"Please fill out {nameof(item.Object.Username)}");
            }
            else
            {
                errorMessagesDict.Add(userNameIndex, [$"Please fill out {nameof(item.Object.Username)}"]);
            }
        }
        if (string.IsNullOrWhiteSpace(item.Object.Password))
        {
            if (errorMessagesDict.ContainsKey(passwordIndex))
            {
                errorMessagesDict[passwordIndex].Add($"Please fill out {nameof(item.Object.Password)}");
            }
            else
            {
                errorMessagesDict.Add(passwordIndex, [$"Please fill out {nameof(item.Object.Password)}"]);
            }
        }

        if (!string.IsNullOrWhiteSpace(item.Object.Title) && item.Object.Title?.Length > 256)
        {
            if (errorMessagesDict.ContainsKey(titleIndex))
            {
                errorMessagesDict[titleIndex].Add("Please make sure all fields are under 256 characters");
            }
            else
            {
                errorMessagesDict.Add(titleIndex, ["Please make sure all fields are under 256 characters"]);
            }
        }
        if (!string.IsNullOrWhiteSpace(item.Object.Username) && item.Object.Username?.Length > 256)
        {
            if (errorMessagesDict.ContainsKey(userNameIndex))
            {
                errorMessagesDict[userNameIndex].Add("Please make sure all fields are under 256 characters");
            }
            else
            {
                errorMessagesDict.Add(userNameIndex, ["Please make sure all fields are under 256 characters"]);
            }
        }
        if (!string.IsNullOrWhiteSpace(item.Object.Password) && item.Object.Password?.Length > 256)
        {
            if (errorMessagesDict.ContainsKey(passwordIndex))
                errorMessagesDict[passwordIndex].Add("Please make sure all fields are under 256 characters");
            else
                errorMessagesDict.Add(passwordIndex, ["Please make sure all fields are under 256 characters"]);
        }

        // Console.WriteLine($"{titleIndex},{userNameIndex},{passwordIndex}");


        if (errorMessagesDict.Any())
        {
            return new GridValidationResponse(Flag: false, errorMessagesDict);
        }

        return new GridValidationResponse(Flag: true, null);
    }
    public List<HandyGridEntity> Items { get; set; }
    public Dictionary<string, RenderFragment<HandyGridEntity>> EditModeFragments { get; set; }
    public Dictionary<string, RenderFragment<HandyGridEntity>> ViewModeFragments { get; set; }
    public IReadOnlyCollection<string> ReadonlyColumns { get; set; }
    public string ExampleFileUploadUrl { get; set; } = "templates/example.csv";
    public Func<IEnumerable<HandyGridEntity>, Task> OnSubmitFile {get;set;}


}

