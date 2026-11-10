using System.Diagnostics;
using System.Text.Json;

public class Messages
{
    private const string DefaultLanguage = "English";
    private const string LanguageFileName = "language_data.json";

    // Private setter: change the language through SetCurrentLanguage so the text is reloaded with it.
    public string CurrentLanguage { get; private set; }

    // Message keys are code identifiers, so they are matched exactly.
    private Dictionary<string, string> _dictionary = new();

    // Lookup maps ignore case, so player input and display names don't need to be lowercased
    // (ToLower() depends on the system culture; e.g. "I" lowercases to "ı" in Turkish).
    private Dictionary<string, string> _raceMap = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> _occupationMap = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> _displayRaceMap = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> _displayOccupationMap = new(StringComparer.OrdinalIgnoreCase);
    private Dictionary<string, string> _displayWeaponMap = new(StringComparer.OrdinalIgnoreCase);

    public Messages()
    {
        CurrentLanguage = DefaultLanguage;
    }

    /// <summary>
    /// Sets the language and immediately loads its text, so CurrentLanguage
    /// always matches the dictionary in use.
    /// </summary>
    public void SetCurrentLanguage(string language)
    {
        CurrentLanguage = language;
        ReadDictionary();
    }

    /// <summary>
    /// Loads all text for CurrentLanguage from language_data.json.
    /// Never throws: problems are reported on stderr and the game keeps running,
    /// falling back to English for an unknown language and to "[key]" placeholders
    /// for anything that couldn't be loaded.
    /// </summary>
    public void ReadDictionary()
    {
        string jsonPath = Path.Combine(AppContext.BaseDirectory, LanguageFileName);

        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(File.ReadAllText(jsonPath));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException)
        {
            ReportProblem($"Could not load '{jsonPath}': {ex.Message}");
            return;
        }

        using (doc)
        {
            JsonElement root = doc.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
            {
                ReportProblem($"'{LanguageFileName}' must contain a JSON object at the top level.");
                return;
            }

            if (!TryGetObject(root, CurrentLanguage, out JsonElement langSection))
            {
                if (CurrentLanguage != DefaultLanguage && TryGetObject(root, DefaultLanguage, out langSection))
                {
                    ReportProblem($"Language '{CurrentLanguage}' not found; falling back to {DefaultLanguage}.");
                    CurrentLanguage = DefaultLanguage;
                }
                else
                {
                    ReportProblem($"Language '{CurrentLanguage}' not found in '{LanguageFileName}'.");
                    return;
                }
            }

            _dictionary = LoadMap(langSection, "dictionary", StringComparer.Ordinal);
            _raceMap = LoadMap(langSection, "raceMap", StringComparer.OrdinalIgnoreCase);
            _occupationMap = LoadMap(langSection, "occupationMap", StringComparer.OrdinalIgnoreCase);
            _displayRaceMap = LoadMap(langSection, "displayRaceMap", StringComparer.OrdinalIgnoreCase);
            _displayOccupationMap = LoadMap(langSection, "displayOccupationMap", StringComparer.OrdinalIgnoreCase);
            _displayWeaponMap = LoadMap(langSection, "displayWeaponMap", StringComparer.OrdinalIgnoreCase);
        }
    }

    public string GetMessage(string key)
    {
        if (_dictionary.TryGetValue(key, out string? message))
            return message;

        // Shown in the debugger's output window so typos and missing translations get noticed.
        Debug.WriteLine($"[Messages] Missing key '{key}' for language '{CurrentLanguage}'.");
        return $"[{key}]";
    }

    public bool IsValidRace(string? input)
    {
        return !string.IsNullOrWhiteSpace(input) && _raceMap.ContainsKey(input.Trim());
    }

    public string NormalizeRace(string input)
    {
        return _raceMap.TryGetValue(input.Trim(), out string? canonical) ? canonical : input;
    }

    public bool IsValidOccupation(string? input)
    {
        return !string.IsNullOrWhiteSpace(input) && _occupationMap.ContainsKey(input.Trim());
    }

    public string NormalizeOccupation(string input)
    {
        return _occupationMap.TryGetValue(input.Trim(), out string? canonical) ? canonical : input;
    }

    public string TranslateRaceForDisplay(string race)
    {
        return _displayRaceMap.TryGetValue(race, out string? translated) ? translated : race;
    }

    public string TranslateOccupationForDisplay(string occupation)
    {
        return _displayOccupationMap.TryGetValue(occupation, out string? translated) ? translated : occupation;
    }

    public string TranslateWeaponForDisplay(string weaponType)
    {
        return _displayWeaponMap.TryGetValue(weaponType, out string? translated) ? translated : weaponType;
    }

    private static bool TryGetObject(JsonElement parent, string name, out JsonElement value)
    {
        return parent.TryGetProperty(name, out value) && value.ValueKind == JsonValueKind.Object;
    }

    /// <summary>
    /// Copies one string-to-string section of the language file into a dictionary.
    /// A missing section gives an empty map; non-string values are skipped. Both are reported.
    /// </summary>
    private Dictionary<string, string> LoadMap(JsonElement langSection, string sectionName, StringComparer comparer)
    {
        var map = new Dictionary<string, string>(comparer);

        if (!TryGetObject(langSection, sectionName, out JsonElement section))
        {
            ReportProblem($"Section '{sectionName}' is missing for language '{CurrentLanguage}'.");
            return map;
        }

        foreach (JsonProperty entry in section.EnumerateObject())
        {
            if (entry.Value.ValueKind == JsonValueKind.String)
                map[entry.Name] = entry.Value.GetString()!;
            else
                ReportProblem($"'{CurrentLanguage}.{sectionName}.{entry.Name}' is not a string; skipped.");
        }

        return map;
    }

    private static void ReportProblem(string problem)
    {
        Console.Error.WriteLine($"[Messages] {problem}");
    }
}
