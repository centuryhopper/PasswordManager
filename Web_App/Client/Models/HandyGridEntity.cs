
using HandyBlazorComponents.Abstracts;
using HandyBlazorComponents.Interfaces;
using Shared.Models;

namespace Client.Models;

public class HandyGridEntity : HandyGridEntityAbstract<PasswordAccountDTO>
{

    public HandyGridEntity() : base()
    {
    }

    public HandyGridEntity(PasswordAccountDTO Object) : base(Object)
    {
    }

    public override object? DisplayPropertyInGrid(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(Object.Id):
                return Object.Id;
            case nameof(Object.Title):
                return Object.Title;
            case nameof(Object.UserId):
                return Object.UserId;
            case nameof(Object.Username):
                return Object.Username;
            case nameof(Object.Password):
                return Object.Password;
            case nameof(Object.CreatedAt):
                return Object.CreatedAt is null ? DateTime.Now.ToString("yyyy-MM-dd") : Object.CreatedAt.GetValueOrDefault().ToString("yyyy-MM-dd");
            case nameof(Object.LastUpdatedAt):
                return Object.LastUpdatedAt is null ? DateTime.Now.ToString("yyyy-MM-dd") : Object.LastUpdatedAt.GetValueOrDefault().ToString("yyyy-MM-dd");
            default:
                throw new Exception("Invalid property name");
        }
    }

    public override int GetPrimaryKey()
    {
        return Object.Id;
    }

    public override void SetPrimaryKey(int id)
    {
        Object.Id = id;
    }
    
    public override void ParsePropertiesFromCSV(Dictionary<string, object> properties)
    {
        base.ParsePropertiesFromCSV(properties);
    }

    public override void SetProperties(Dictionary<string, object> properties)
    {
        base.SetProperties(properties);
    }

}

