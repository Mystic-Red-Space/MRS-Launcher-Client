﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace MRSLauncherClient
{
    public class PageManager
    {
        public List<Page> PageList { get; private set; }

        public PageManager()
        {
            PageList = new List<Page>();
        }

        public Page GetContent(string name)
        {
            foreach (var item in PageList)
            {
                if (item.Name == name)
                    return item;
            }

            // 못 찾았을때
            return null;
        }

        public Page GetContent(int index)
        {
            return PageList[index];
        }
    }
}
