namespace Eclipse.Engine.Utils.Load.Assets
{
    internal abstract class Asset
    {
        internal string Name { get; set; }
        internal bool isLoaded { get; set; }


        //protected AssetManager Content;
        internal Asset()
        {
            isLoaded = false;
        }
    }
}
