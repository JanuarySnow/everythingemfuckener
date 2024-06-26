using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using System.Threading.Tasks;
using Mutagen.Bethesda.Plugins;
using System.Drawing;
using Mutagen.Bethesda.Oblivion;
using IRace = Mutagen.Bethesda.Skyrim.IRace;
using IPlacedObject = Mutagen.Bethesda.Skyrim.IPlacedObject;
using Mutagen.Bethesda.Fallout4;
using IColorRecord = Mutagen.Bethesda.Skyrim.IColorRecord;
using IOutfit = Mutagen.Bethesda.Skyrim.IOutfit;

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

            List<String> playableraces = new List<String>();
            List<FormLink<IRace>> listOfRaces = new List<FormLink<IRace>>();
            List<String> listofNames = new List<String>();
            List<FormLink<IPlacedObject>> ListofDoors = new List<FormLink<IPlacedObject>>();
            List<FormLink<IColorRecord>> ListofColors = new List<FormLink<IColorRecord>>();
            List<FormLink<IOutfit>> ListofOutfits = new List<FormLink<IOutfit>>();

            playableraces.Add("BretonRace");
            playableraces.Add("DefaultRace");
            playableraces.Add("DarkElfRace");
            playableraces.Add("HighElfRace");
            playableraces.Add("ImperialRace");
            playableraces.Add("OrcRace");
            playableraces.Add("RedguardRace");
            playableraces.Add("WoodElfRace");
            playableraces.Add("NordRaceVampire");
            playableraces.Add("BretonRaceVampire");
            playableraces.Add("DarkElfRaceVampire");
            playableraces.Add("HighElfRaceVampire");
            playableraces.Add("ImperialRaceVampire");
            playableraces.Add("OrcRaceVampire");
            playableraces.Add("RedguardRaceVampire");
            playableraces.Add("WoodElfRaceVampire");


            foreach (var npc in state.LoadOrder.PriorityOrder.OnlyEnabled().Npc().WinningOverrides())
            {
                if (npc != null && npc.EditorID != null) {
                    var race = npc.Race.TryResolve(state.LinkCache);
                    if(race == null) { continue; }
                    var raceid = race.EditorID;
                    if(raceid == null) { continue; }
                    if (playableraces.Contains(raceid.ToString()))
                    {
                        var getoutfit = npc.DefaultOutfit.TryResolve(state.LinkCache);
                        if (getoutfit == null) { continue; }
                        ListofOutfits.Add(npc.DefaultOutfit.FormKey);
                        Console.WriteLine(raceid.ToString());
                    }
                }
            }
            foreach (var npc in state.LoadOrder.PriorityOrder.OnlyEnabled().Npc().WinningOverrides())
            {
                if (npc != null && npc.EditorID != null)
                {
                    var race = npc.Race.TryResolve(state.LinkCache);
                    if (race == null) { continue; }
                    var raceid = race.EditorID;
                    if (raceid == null) { continue; }
                    if (playableraces.Contains(raceid))

                    {
                        var npc_override = state.PatchMod.Npcs.GetOrAddAsOverride(npc);
                        var random = new Random();
                        int index = random.Next(listOfRaces.Count);
                        int rand_outfit = random.Next(ListofOutfits.Count);

                        var outfitold = npc.DefaultOutfit.TryResolve(state.LinkCache);
                        if (outfitold == null) { continue; }
                        if (outfitold.EditorID != null)
                        {
                            npc_override.DefaultOutfit.SetTo(ListofOutfits[rand_outfit]);
                            int another_one = random.Next(ListofOutfits.Count);
                            npc_override.SleepingOutfit.SetTo(ListofOutfits[another_one]);
                        }
                    }
                    

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
            /*foreach (var placedobjectgetter in state.LoadOrder.PriorityOrder.OnlyEnabled().PlacedObject().WinningContextOverrides(state.LinkCache)) {
                if (placedobjectgetter.Record.MajorRecordFlagsRaw == 0x0000_0800) continue;
                var random = new Random();
                int rand1 = random.Next(10);
                if( rand1 == 0)
                {
                    if (placedobjectgetter.Record.EditorID == null)
                    {
                        IPlacedObject modifiedObject = placedobjectgetter.GetOrAddAsOverride(state.PatchMod);
                        modifiedObject.Scale = NextFloat((float)0.85, (float)1.15);
                    }
                }
            }*/
        }
    }
}
