using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace FugitiveModel
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Role
    {
        Fugitive,
        Marshal
    }
}
