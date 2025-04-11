using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Forms;  // Add this for Control and MethodInvoker
using WindowsInput;
using WindowsInput.Native;
using FridayNightFunkin;
using FNFBot20;  // Add this to ensure PsychEngineNoteChecker is visible

namespace FNFBot20
{
    public class Bot
    {
        public static bool Playing = false;
        
        public static Stopwatch watch { get; set; }
        
        // Add static properties for last loaded song and bot instance
        public static string LastLoadedSong { get; private set; }
        public static Bot Instance { get; private set; }
        
        public string sngDir { get; set; }
        public KeyBot kBot;
        public MapBot mBot;
        public RenderBot rBot;

        public InputSimulator simulator = new InputSimulator();
        
        public Thread currentPlayThread { get; set; }
        
        public Bot()
        {
            // Create keyhooks with KeyBot
            kBot = new KeyBot();
            kBot.InitHooks();
            Instance = this;
        }
        
        public void Load(string songDirectory)
        {
            Form1.WriteToConsole("attempting to load " + songDirectory);
            if (!File.Exists(songDirectory))
            {
                Form1.WriteToConsole("Path doesn't exist");
                return;
            }

            try
            {
                LastLoadedSong = songDirectory;
                
                // Process and clean the song file first
                string processedFile = PsychEngineNoteChecker.ProcessSongFile(songDirectory);
                
                currentPlayThread?.Abort();
                Form1.currentThreads?.Remove(currentPlayThread);

                sngDir = processedFile;
                
                // basic loading
                // get a FNFSong through map bot
                mBot = new MapBot(processedFile);

                // Create the render bot (renders notes to the screen)
                rBot = new RenderBot((int) mBot.song.Bpm);
                
                currentPlayThread = new Thread(PlayThread);
                currentPlayThread.Start();
                
                Form1.currentThreads?.Add(currentPlayThread);
                
                Form1.WriteToConsole("Loaded "  + mBot.song.SongName + " with " + mBot.song.Sections.Count + " sections.");

                watch = new Stopwatch();
                
                Form1.offset.Text = "Offset: " + kBot.offset;
            }
            catch (Exception ex)
            {
                Form1.WriteToConsole($"Failed to load song: {ex.Message}");
            }
        }
        
        private int notesPlayed = 0;
        private void PlayThread()
        {
            Form1.WriteToConsole("Play Thread created...");
            try
            {
                while (true)
                {
                    if (!watch.IsRunning && Playing)
                    {
                        Form1.watchTime.Text = "Time: 0";
                        watch.Start();
                        notesPlayed = 0;  // Reset notes counter when starting
                        Form1.SectionSee = 1;  // Reset section counter
                    }
                    else if (!Playing && watch.IsRunning)
                    {
                        Form1.console.Text = "";
                        watch.Reset();
                        notesPlayed = 0;  // Reset notes counter when stopping
                        Form1.pnlField.Controls.Clear();  // Clear rendered notes
                    }

                    if (!Playing)
                    {
                        Thread.Sleep(100);
                        continue;   
                    }

                    int sectionSee = 0;
                    var activeThreads = new List<Thread>();

                    foreach (FNFSong.FNFSection sect in mBot.song.Sections)
                    {
                        if (!Playing) 
                        {
                            notesPlayed = 0;  // Reset counter when stopping
                            break;
                        }
                        
                        sectionSee++;
                        List<FNFSong.FNFNote> notesToPlay = mBot.GetHitNotes(sect);

                        foreach (FNFSong.FNFNote n in notesToPlay)
                        {
                            if (!Playing) break;
                            
                            Thread t = new Thread(() => HandleNote(n));
                            Form1.currentThreads.Add(t);
                            activeThreads.Add(t);
                            t.Start();
                        }

                        if (Form1.Rendering)
                        {
                            Thread list = new Thread(() => rBot.ListNotes(notesToPlay));
                            Form1.currentThreads.Add(list);
                            activeThreads.Add(list);
                            list.Start();
                        }

                        while (notesPlayed != notesToPlay.Count && sectionSee == Form1.SectionSee && Playing)
                        {
                            Form1.watchTime.Text = "Time: " + watch.Elapsed.TotalSeconds.ToString();
                            Thread.Sleep(1);
                        }

                        // Clean up completed threads
                        activeThreads.RemoveAll(t => !t.IsAlive);

                        if (!Playing) 
                        {
                            // Clean up when stopping
                            foreach (var thread in activeThreads)
                            {
                                if (thread.IsAlive)
                                    thread.Join(100);
                            }
                            Form1.pnlField.Controls.Clear();
                            break;
                        }

                        Form1.WriteToConsole("Section See: " + sectionSee);
                        
                        if (sectionSee == Form1.SectionSee)
                        {
                            notesPlayed = 0;
                            Form1.WriteToConsole("---");
                            sectionSee = 0;
                        }
                    }

                    // Wait for remaining threads to complete
                    foreach (var thread in activeThreads)
                    {
                        if (thread.IsAlive)
                            thread.Join(100); // Wait up to 100ms for each thread
                    }

                    if (!Playing)
                    {
                        Form1.console.Text = "";
                        Form1.WriteToConsole("Stopped!");
                        continue;
                    }

                    Form1.console.Text = "";
                    Playing = false;
                    Form1.WriteToConsole("Completed!");
                }
            }
            catch (ThreadAbortException)
            {
                // Clean shutdown
                Playing = false;
                watch.Stop();
                Form1.pnlField.Controls.Clear();
                notesPlayed = 0;
            }
            catch (Exception e)
            {
                Form1.WriteToConsole("Exception on Play Thread\n" + e);
            }
        }

        private bool[] boolAr = new[] {false, false, false, false};
        
        public void HandleNote(FNFSong.FNFNote n)
        {
            while (watch.Elapsed.TotalMilliseconds < (double) n.Time - kBot.offset)
            {
                Thread.Sleep(1);
                if (!Playing)
                    return;
            }
            
            bool shouldHold = n.Length > 0;

            var noteThread = new Thread(() =>
            {
                try 
                {
                    switch (n.Type)
                    {
                        case FNFSong.NoteType.Left:
                        case FNFSong.NoteType.RLeft:
                            if (shouldHold)
                            {
                                simulator.Keyboard.KeyDown(VirtualKeyCode.LEFT);
                                Thread.Sleep(Convert.ToInt32(n.Length));
                                simulator.Keyboard.KeyUp(VirtualKeyCode.LEFT);
                            }
                            else
                            {
                                kBot.KeyPress(0x25, 0x1e);
                            }
                            break;
                        case FNFSong.NoteType.Down:
                        case FNFSong.NoteType.RDown:
                            if (shouldHold)
                            {
                                simulator.Keyboard.KeyDown(VirtualKeyCode.DOWN);
                                Thread.Sleep(Convert.ToInt32(n.Length));
                                simulator.Keyboard.KeyUp(VirtualKeyCode.DOWN);
                            }
                            else
                                kBot.KeyPress(0x28, 0x1f);
                            break;
                        case FNFSong.NoteType.Up:
                        case FNFSong.NoteType.RUp:
                            if (shouldHold)
                            {
                                simulator.Keyboard.KeyDown(VirtualKeyCode.UP);
                                Thread.Sleep(Convert.ToInt32(n.Length));
                                simulator.Keyboard.KeyUp(VirtualKeyCode.UP);
                            }
                            else
                                kBot.KeyPress(0x26, 0x11);
                            break;
                        case FNFSong.NoteType.Right:
                        case FNFSong.NoteType.RRight:
                            if (shouldHold)
                            {
                                simulator.Keyboard.KeyDown(VirtualKeyCode.RIGHT);
                                Thread.Sleep(Convert.ToInt32(n.Length));
                                simulator.Keyboard.KeyUp(VirtualKeyCode.RIGHT);
                            }
                            else
                                kBot.KeyPress(0x27, 0x20);
                            break;
                    }
                }
                catch (ThreadAbortException)
                {
                    simulator.Keyboard.KeyUp(VirtualKeyCode.LEFT);
                    simulator.Keyboard.KeyUp(VirtualKeyCode.DOWN);
                    simulator.Keyboard.KeyUp(VirtualKeyCode.UP);
                    simulator.Keyboard.KeyUp(VirtualKeyCode.RIGHT);
                }
            });
            
            Form1.currentThreads.Add(noteThread);
            noteThread.Start();
            notesPlayed++;
        }

        // Add method to update offset without full reload
        public void UpdateOffset(int newOffset)
        {
            if (mBot == null) return;  // No song loaded

            // Just update the offset value
            kBot.offset = newOffset;
            Form1.offset.Text = "Offset: " + newOffset;

            // Clear only old notes from the display
            if (Form1.pnlField.InvokeRequired)
            {
                Form1.pnlField.BeginInvoke((MethodInvoker) delegate
                {
                    var controlsToRemove = new List<Control>();
                    foreach (Control control in Form1.pnlField.Controls)
                    {
                        var yPos = control.Location.Y;
                        if (yPos < 0 || yPos > Form1.pnlField.Height)
                        {
                            controlsToRemove.Add(control);
                        }
                    }
                    foreach (var control in controlsToRemove)
                    {
                        Form1.pnlField.Controls.Remove(control);
                    }
                });
            }
            else
            {
                var controlsToRemove = new List<Control>();
                foreach (Control control in Form1.pnlField.Controls)
                {
                    var yPos = control.Location.Y;
                    if (yPos < 0 || yPos > Form1.pnlField.Height)
                    {
                        controlsToRemove.Add(control);
                    }
                }
                foreach (var control in controlsToRemove)
                {
                    Form1.pnlField.Controls.Remove(control);
                }
            }
        }
    }
}