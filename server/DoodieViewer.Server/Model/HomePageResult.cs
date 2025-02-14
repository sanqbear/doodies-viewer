using DoodieViewer.Server.Model.Items;

namespace DoodieViewer.Server.Model
{
    public class HomePageResult
    {
        public HomePageResult()
        {
            Recent = Enumerable.Empty<ManhwaBase>();
            Ranked = Enumerable.Empty<ManhwaBase>();
            Weekly = Enumerable.Empty<ManhwaBase>();
        }

        public HomePageResult(IEnumerable<ManhwaBase> recent, IEnumerable<ManhwaBase> ranked, IEnumerable<ManhwaBase> weekly)
        {
            Recent = recent;
            Ranked = ranked;
            Weekly = weekly;
        }

        public IEnumerable<ManhwaBase> Recent { get; private set; }

        public IEnumerable<ManhwaBase> Ranked { get; private set; }

        public IEnumerable<ManhwaBase> Weekly { get; private set; }
    }
}
