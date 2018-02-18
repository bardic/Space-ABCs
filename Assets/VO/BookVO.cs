using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BookVO
{
    public PageVO[] pages;

    [Serializable]
    public class PageVO
    {
        public string letter;
        public string name;
        public string image;
        public string description;
        public string phonetic;
    }
}
