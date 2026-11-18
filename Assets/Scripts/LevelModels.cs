using System.Collections.Generic;
using UnityEngine;

namespace TurnTheOrientation
{
    public sealed class BoxSpec
    {
        public BoxSpec(Vector2 position, Vector2 size, string style = "wall")
        {
            Position = position;
            Size = size;
            Style = style;
        }

        public Vector2 Position { get; }
        public Vector2 Size { get; }
        public string Style { get; }
    }

    public sealed class CrateSpec
    {
        public CrateSpec(Vector2 position)
        {
            Position = position;
        }

        public Vector2 Position { get; }
    }

    public sealed class PlateSpec
    {
        public PlateSpec(Vector2 position)
        {
            Position = position;
        }

        public Vector2 Position { get; }
    }

    public sealed class GateSpec
    {
        public GateSpec(Vector2 position, Vector2 size)
        {
            Position = position;
            Size = size;
        }

        public Vector2 Position { get; }
        public Vector2 Size { get; }
    }

    public sealed class LevelDefinition
    {
        public LevelDefinition(
            string title,
            string hint,
            Vector2 spawn,
            Vector2 exit,
            GateSpec gate,
            IReadOnlyList<BoxSpec> boxes,
            IReadOnlyList<CrateSpec> crates,
            IReadOnlyList<PlateSpec> plates)
        {
            Title = title;
            Hint = hint;
            Spawn = spawn;
            Exit = exit;
            Gate = gate;
            Boxes = boxes;
            Crates = crates;
            Plates = plates;
        }

        public string Title { get; }
        public string Hint { get; }
        public Vector2 Spawn { get; }
        public Vector2 Exit { get; }
        public GateSpec Gate { get; }
        public IReadOnlyList<BoxSpec> Boxes { get; }
        public IReadOnlyList<CrateSpec> Crates { get; }
        public IReadOnlyList<PlateSpec> Plates { get; }
    }
}
