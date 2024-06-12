using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Threading.Tasks;
using Mutagen.Bethesda.Plugins;
using System.Drawing;

namespace Theeverythingemfuckener
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetTypicalOpen(GameRelease.SkyrimSE, "YourPatcher.esp")
                .Run(args);
        }


        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            static float NextFloat(float min, float max)
            {
                System.Random random = new System.Random();
                double val = (random.NextDouble() * (max - min) + min);
                return (float)val;
            }

            List<FormLink<IRace>> listOfRaces = new List<FormLink<IRace>>();
            List<String> listofNames = new List<String>();
            List<FormLink<IPlacedObject>> ListofDoors = new List<FormLink<IPlacedObject>>();
            List<FormLink<IColorRecord>> ListofColors = new List<FormLink<IColorRecord>>();
            List<FormLink<IVoiceType>> ListofVoices = new List<FormLink<IVoiceType>>();
            List<FormLink<IOutfit>> ListofOutfits = new List<FormLink<IOutfit>>();
            foreach (var race_record in state.LoadOrder.PriorityOrder.OnlyEnabled().Race().WinningOverrides()) {
                if (race_record.EditorID == null)
                {
                    continue;
                }
                listOfRaces.Add(race_record.FormKey);
            }
            foreach( var outfit_record in state.LoadOrder.PriorityOrder.OnlyEnabled().Outfit().WinningOverrides())
            { 
                if ( outfit_record.EditorID != null)
                {
                    ListofOutfits.Add(outfit_record.FormKey);
                }
            }
            foreach (var npc in state.LoadOrder.PriorityOrder.OnlyEnabled().Npc().WinningOverrides())
            {
                if (npc != null && npc.EditorID != null) {
                    if (npc.Voice != null)
                    {
                        ListofVoices.Add(npc.Voice.FormKey);
                    }
                }
            }
            foreach (var npc in state.LoadOrder.PriorityOrder.OnlyEnabled().Npc().WinningOverrides())
            {
                if (npc != null && npc.EditorID != null)
                {
                    var npc_override = state.PatchMod.Npcs.GetOrAddAsOverride(npc);
                    var random = new Random();
                    int index = random.Next(listOfRaces.Count);
                    int randvoice = random.Next(ListofVoices.Count);
                    int rand_outfit = random.Next(ListofOutfits.Count);

                    int rand1 = random.Next(10);
                    if( rand1 == 0)
                    {
                        npc_override.Race.SetTo(listOfRaces[index]);
                    }
                    npc_override.Voice.SetTo(ListofVoices[randvoice]);
                    npc_override.DefaultOutfit.SetTo(ListofOutfits[rand_outfit]);
                    int another_one = random.Next(ListofOutfits.Count);
                    npc_override.SleepingOutfit.SetTo(ListofOutfits[another_one]);

                }
            }
            foreach( var colorgetter in state.LoadOrder.PriorityOrder.ColorRecord().WinningOverrides())
            {
                var rnd = new Random();
                var r_rnd = rnd.Next(0, 255);
                var g_rnd = rnd.Next(0, 255);
                var b_rnd = rnd.Next(0, 255);
                Color c = Color.FromArgb(255, r_rnd, g_rnd, b_rnd);
                var color_override = state.PatchMod.Colors.GetOrAddAsOverride(colorgetter);
                color_override.Color = c;
            }
            int inc = 0;
            foreach (var placedobjectgetter in state.LoadOrder.PriorityOrder.OnlyEnabled().PlacedObject().WinningContextOverrides(state.LinkCache)) {
                if (placedobjectgetter.Record.MajorRecordFlagsRaw == 0x0000_0800) continue;
                if (placedobjectgetter.Record.EditorID == null)
                {
                    Console.WriteLine("rescaling thing" + inc);
                    IPlacedObject modifiedObject = placedobjectgetter.GetOrAddAsOverride(state.PatchMod);
                    modifiedObject.Scale = NextFloat((float)0.95, (float)1.05);
                }

                inc += 1;
            }
        }
    }
}
