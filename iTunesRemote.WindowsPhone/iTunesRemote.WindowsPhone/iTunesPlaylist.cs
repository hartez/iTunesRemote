namespace iTunesRemote.WindowsPhone
{
    public class iTunesPlaylist
    {
        public iTunesPlaylist(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}