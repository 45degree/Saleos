
namespace Saleos.Views.Shared
{
    public static class Pagination
    {
        public static string Disabled(int CurrentPage, int MaxPage)
        {
            if (CurrentPage < 1 || CurrentPage > MaxPage) return "disabled";
            else return null;
        }
    }
}
