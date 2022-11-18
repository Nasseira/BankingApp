using System.Collections.Generic;

namespace BankApp.Model
{
    class CheckedCategory
    {
        private Category _category;

        public Category Category
        {
            get { return _category; }
            set { _category = value; }
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; }
        }

        public CheckedCategory() { }
        public CheckedCategory(Category category, bool isChecked)
        {
            Category = category;
            IsChecked = isChecked;
        }

        public static List<CheckedCategory> CheckedCategoriesList()
        {
            List<CheckedCategory> checkedCategories = new List<CheckedCategory>();
            checkedCategories.Add(new CheckedCategory(new Category("<No Category>"), true));
            List<Category> AllCategories = Category.GetAll();
            foreach (var category in AllCategories)
            {
                checkedCategories.Add(new CheckedCategory(category, true));
            }

            return checkedCategories;
        }


    }
}
