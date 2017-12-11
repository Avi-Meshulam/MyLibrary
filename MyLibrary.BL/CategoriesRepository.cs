using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLibrary.BL
{
    public static class CategoriesRepository
    {
        public static Dictionary<Category, List<Enum>> Categories { get; private set; }

        static CategoriesRepository()
        {
            FillCategories();
        }

        private static void FillCategories()
        {
            Categories = new Dictionary<Category, List<Enum>>();

            string nameSpace = typeof(Category).Namespace;
            Array categories = typeof(Category).GetEnumValues();

            foreach (var category in categories)
            {
                var subCategory = Type.GetType($"{nameSpace}.{category}");
                var subCategories = subCategory?.GetValues().ToList();
                Categories.Add((Category)category, subCategories);
            }
        }

        public static IEnumerable<KeyValuePair<Enum, Enum>> GetSubCategoriesAsKeyValuePairs(IEnumerable<Enum> categories = null)
        {
            var query = categories == null ? 
                Categories : Categories.Where(i => categories.Contains(i.Key));

            return query.SelectMany(i => 
                i.Value?.Select(e => new KeyValuePair<Enum, Enum>(i.Key, e)) ??
                    new List<KeyValuePair<Enum, Enum>> { new KeyValuePair<Enum, Enum> (i.Key, null)});
        }

        public static IEnumerable<Enum> GetSubCategories(IEnumerable<Enum> categories = null)
        {
            var query = categories == null ?
                Categories : Categories.Where(i => categories.Contains(i.Key));

            return query.SelectMany(item => item.Value ?? new List<Enum> { null });
        }
    }
}
