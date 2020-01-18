using Asm;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Editor
{
    public static class Globals
    {
        public static Program program;

        public static List<Draggable.Slot> dividers = new List<Draggable.Slot>();
        public static List<Draggable.Slot> slotFields = new List<Draggable.Slot>();
        public static List<Draggable.Slot> trashSlots = new List<Draggable.Slot>();

        public static void Init(Program program)
        {
            Globals.program = program;

            dividers.Clear();
            slotFields.Clear();
            trashSlots.Clear();
        }

        public static void Reset()
        {
            dividers.Clear();
            slotFields.Clear();
        }
    }
}