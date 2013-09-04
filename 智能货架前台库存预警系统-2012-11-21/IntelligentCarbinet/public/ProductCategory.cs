using System;
using System.Collections.Generic;
using System.Text;

namespace ProductCategoryManage
{
    public class ProductCategory
    {
        public string category_id;
        public string category_name;
        public string category_image;
        public string state;
        public ProductCategory()
        {

        }
        public ProductCategory(string id, string name, string image)
        {
            this.category_id = id;
            this.category_name = name;
            this.category_image = image;
        }
    }
}
