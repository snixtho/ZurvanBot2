namespace ZurvanBot.Discord.Resources.Objects
{
    public class EmbedObject: ResObject
    {
        public string title;
        public string type;
        public string description;
        public string url;
        public string timestamp;
        public int? color;
        public EmbedFooterObject footer;
        public EmbedImageObject image;
        public EmbedThumbnailObject thumbnail;
        public EmbedVideoObject video;
        public EmbedProviderObject provider;
        public EmbedAuthorObject author;
        public EmbedFieldObject[] fields;
    }
}