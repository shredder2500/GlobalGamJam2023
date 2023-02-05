﻿using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Resources;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using static GameJam.Config;

namespace GameJam.Systems;

public class WaterConsumption : ISystem, IDisposable
{
    private readonly ILogger _logger;
    private readonly IResourceManager _resources;
    private readonly IWorld _world;
    private readonly SpriteSheet _spriteSheet;

    public WaterConsumption(IResourceManager resources, IWorld world, ILogger<WaterConsumption> logger)
    {
        _logger = logger;
        _resources = resources;
        _world = world;
        _spriteSheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), StumpyTileSheetSize,
            new(PPU, PPU));
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stumpy-tileset");
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        // Remove water component if there is a root at same location
        var search = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<Root>() && x.HasComponent<Position>())
            .Select(x => x.GetIndices().Select(i => x.GetComponent<Position>(i)))
            .SelectMany(x => x);

        var water = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<Water>() && x.HasComponent<Position>())
            .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<Position>(i))))
            .SelectMany(x => x).Where(x =>
            {
                return search.Contains(x.Item2);
            }); 

        foreach (var (entity, _) in water) 
        {
            _world.RemoveComponent<Water>(entity);
            _world.SetComponent(entity, _spriteSheet.GetSprite(6));
            IncreaseEnergy(4);
        }

        void IncreaseEnergy(int amount)
        {
            var player = _world.GetEntityBuckets()
                .Where(x => x.HasComponent<EnergyManagement>())
                .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<EnergyManagement>(i))))
                .SelectMany(x => x)
                .FirstOrDefault();

            if (player.Item2 != null)
            {
                int currentEnergy = player.Item2.Value;
                _world.SetComponent(player.Item1, new EnergyManagement(currentEnergy + amount));
            }
        }

        return ValueTask.CompletedTask; 
    }
}
