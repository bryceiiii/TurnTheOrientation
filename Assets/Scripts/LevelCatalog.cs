using System.Collections.Generic;
using UnityEngine;

namespace TurnTheOrientation
{
    public static class LevelCatalog
    {
        private static readonly BoxSpec[] Border =
        {
            new BoxSpec(new Vector2(-8.4f, 0f), new Vector2(0.8f, 12f)),
            new BoxSpec(new Vector2(8.4f, 0f), new Vector2(0.8f, 12f)),
            new BoxSpec(new Vector2(0f, 5.8f), new Vector2(17.6f, 0.8f)),
            new BoxSpec(new Vector2(0f, -5.8f), new Vector2(17.6f, 0.8f))
        };

        private static readonly IReadOnlyList<LevelDefinition> Levels =
            new[]
            {
                new LevelDefinition(
                    "FIRST TURN",
                    "Push the core onto the socket. Latch the door, then turn the room.",
                    new Vector2(-6.4f, -3.7f),
                    new Vector2(6.2f, 4.2f),
                    new GateSpec(new Vector2(0.7f, -3.7f), new Vector2(0.65f, 1.9f)),
                    Combine(
                        Border,
                        new[]
                        {
                            new BoxSpec(new Vector2(-3.3f, 2.7f), new Vector2(0.7f, 4.7f)),
                            new BoxSpec(new Vector2(2.4f, 1.4f), new Vector2(0.7f, 6.1f)),
                            new BoxSpec(new Vector2(5.3f, -2.1f), new Vector2(0.7f, 3.5f)),
                            new BoxSpec(new Vector2(-1.1f, 4.2f), new Vector2(4.5f, 0.65f)),
                            new BoxSpec(new Vector2(3.9f, -3.9f), new Vector2(3.3f, 0.65f))
                        }),
                    new[] { new CrateSpec(new Vector2(-4.7f, -3.7f)) },
                    new[] { new PlateSpec(new Vector2(-1.8f, -3.7f)) }),

                new LevelDefinition(
                    "TWO SIGNALS",
                    "Two cores, one circuit. Both sockets must stay lit.",
                    new Vector2(-6.6f, 0f),
                    new Vector2(6.4f, 0f),
                    new GateSpec(new Vector2(1.5f, 0f), new Vector2(0.65f, 3.1f)),
                    Combine(
                        Border,
                        new[]
                        {
                            new BoxSpec(new Vector2(-2.9f, 3.2f), new Vector2(0.7f, 3.8f)),
                            new BoxSpec(new Vector2(-2.9f, -3.2f), new Vector2(0.7f, 3.8f)),
                            new BoxSpec(new Vector2(3.8f, 2.9f), new Vector2(0.7f, 4.4f)),
                            new BoxSpec(new Vector2(3.8f, -2.9f), new Vector2(0.7f, 4.4f)),
                            new BoxSpec(new Vector2(2.6f, -5f), new Vector2(2.2f, 0.65f)),
                            new BoxSpec(new Vector2(6.1f, 2.1f), new Vector2(3.4f, 0.65f)),
                            new BoxSpec(new Vector2(-0.2f, 4.6f), new Vector2(4.4f, 0.65f))
                        }),
                    new[]
                    {
                        new CrateSpec(new Vector2(-5.2f, 2.6f)),
                        new CrateSpec(new Vector2(-5.2f, -2.6f))
                    },
                    new[]
                    {
                        new PlateSpec(new Vector2(-1.3f, 2.6f)),
                        new PlateSpec(new Vector2(-1.3f, -2.6f))
                    }),

                new LevelDefinition(
                    "CROSS CURRENT",
                    "A side route can deliver what the floor cannot. Turn twice if you need it.",
                    new Vector2(-6.4f, 4.2f),
                    new Vector2(5.9f, -4.4f),
                    new GateSpec(new Vector2(1.7f, -4.2f), new Vector2(0.65f, 2.2f)),
                    Combine(
                        Border,
                        new[]
                        {
                            new BoxSpec(new Vector2(-3.7f, -0.2f), new Vector2(0.7f, 8.2f)),
                            new BoxSpec(new Vector2(0.2f, 2.8f), new Vector2(5.5f, 0.65f)),
                            new BoxSpec(new Vector2(3.2f, -0.4f), new Vector2(0.7f, 9.2f)),
                            new BoxSpec(new Vector2(5.4f, 3.4f), new Vector2(2.6f, 0.65f)),
                            new BoxSpec(new Vector2(-0.9f, -4.8f), new Vector2(4.7f, 0.65f)),
                            new BoxSpec(new Vector2(6.4f, -1.2f), new Vector2(2.8f, 0.65f)),
                            new BoxSpec(new Vector2(-5.9f, -2.8f), new Vector2(3.0f, 0.65f))
                        }),
                    new[]
                    {
                        new CrateSpec(new Vector2(-5.3f, -1.5f)),
                        new CrateSpec(new Vector2(4.4f, 4.2f))
                    },
                    new[]
                    {
                        new PlateSpec(new Vector2(-1.0f, -1.5f)),
                        new PlateSpec(new Vector2(4.7f, -1.0f))
                    })
            };

        public static IReadOnlyList<LevelDefinition> All => Levels;

        private static IReadOnlyList<BoxSpec> Combine(BoxSpec[] border, BoxSpec[] interior)
        {
            var result = new List<BoxSpec>(border.Length + interior.Length);
            result.AddRange(border);
            result.AddRange(interior);
            return result;
        }
    }
}
