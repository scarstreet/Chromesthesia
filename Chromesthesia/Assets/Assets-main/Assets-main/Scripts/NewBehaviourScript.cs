using System;
using System.Collections.Generic;

class GlobalData
{
    public Dictionary<string, int> Scenes = new Dictionary<string, int>();
    GlobalData() {
        Scenes.Add("-", 0); // TODO - do all this
    }
};