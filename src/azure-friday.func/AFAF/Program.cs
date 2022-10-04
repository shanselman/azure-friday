using AFAF;

List<Episode> episodes = await DocsToDump.GetEpisodeList();

using FileStream fs1 = File.Open("dump.xml",FileMode.Create, FileAccess.Write);
await DocsToDump.DumpDoc(fs1, episodes, Format.Rss);

using FileStream fs2 = File.Open("dump.json",FileMode.Create, FileAccess.Write);
await DocsToDump.DumpDoc(fs2, episodes, Format.Json);

using FileStream fs3 = File.Open("dumpaudio.xml", FileMode.Create, FileAccess.Write);
await DocsToDump.DumpDoc(fs3, episodes, Format.RssAudio);