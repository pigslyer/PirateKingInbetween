using Godot;

using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Gary.Dialogue
{
    public class DialogueInstance
    {
        public uint Counter {get; private set;} = 0;

        public uint GetId() => Counter++;
    }
}