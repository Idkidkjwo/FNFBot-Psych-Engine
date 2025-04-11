using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace FNFBot20
{
    public class PsychEngineNoteChecker
    {
        public static string ProcessSongFile(string filePath)
        {
            try
            {
                // Read and parse the JSON file
                string jsonContent = File.ReadAllText(filePath);
                JObject songData = JObject.Parse(jsonContent);

                // Clean the special notes
                CheckAndCleanSpecialNotes(songData);

                // Get song name and difficulty from the file name
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string[] parts = fileName.Split('-');
                string songName = parts[0];
                string difficulty = parts.Length > 1 ? parts[parts.Length - 1] : "normal";

                // Create the output filename - preserve the original difficulty name
                string directory = Path.GetDirectoryName(filePath);
                string outputFileName = $"saved_{fileName}.json";
                string outputPath = Path.Combine(directory, outputFileName);

                // Save the modified JSON
                string outputJson = songData.ToString(Formatting.Indented);
                File.WriteAllText(outputPath, outputJson);

                Console.WriteLine($"Successfully saved cleaned song file to: {outputPath}");
                return outputPath;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing song file: {ex.Message}");
                throw; // Rethrow to handle in the Bot class
            }
        }

        public static string GetDifficultyFromPath(string filePath)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePath).ToLower();
            string[] parts = fileName.Split('-');
            if (parts.Length > 1)
            {
                // Return the actual difficulty/variant name from the file
                return parts[parts.Length - 1];
            }
            return "normal"; // default if no difficulty specified
        }

        public static void CheckAndCleanSpecialNotes(JObject songData)
        {
            if (songData == null || !songData.ContainsKey("song") || 
                !(songData["song"] is JObject songObj) || !songObj.ContainsKey("notes"))
                return;

            var sections = songObj["notes"] as JArray;
            if (sections == null)
                return;

            foreach (JObject section in sections)
            {
                if (!section.ContainsKey("sectionNotes"))
                    continue;

                var sectionNotes = section["sectionNotes"] as JArray;
                if (sectionNotes == null)
                    continue;

                for (int i = 0; i < sectionNotes.Count; i++)
                {
                    var note = sectionNotes[i] as JArray;
                    if (note == null || note.Count < 3)
                        continue;

                    // Clean up note duration (3rd element)
                    if (note[2] != null)
                    {
                        double duration = 0;
                        if (!double.TryParse(note[2].ToString(), out duration))
                        {
                            // If duration is invalid, set it to 0 (regular note)
                            note[2] = 0;
                        }
                    }

                    // Check if note has a special property (4th element)
                    if (note.Count > 3)
                    {
                        // Remove the special property by creating a new array with only the first 3 elements
                        var newNote = new JArray(
                            note[0], // time
                            note[1], // noteType
                            note[2]  // duration
                        );
                        sectionNotes[i] = newNote;
                    }
                }
            }
        }
    }
} 