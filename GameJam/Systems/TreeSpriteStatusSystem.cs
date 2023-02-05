﻿using GameJam.Components;
using GameJam.Engine.Rendering;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameJam.Systems;

public class TreeSpriteStatusSystem : ISystem, IDisposable
{
    private readonly IWorld _world;
    private readonly SpriteSheet _spritesheet;
    private readonly IResourceManager _resources;
    private readonly Sprite[] _treeDeadSprites;

    public TreeSpriteStatusSystem(IWorld world, IResourceManager resources)
    {
        _world = world;
        _resources = resources;
        _spritesheet = new(resources.Load<Texture>("sprite.stumpy-tileset"), new(320, 128),
                new(16, 16));
        _treeDeadSprites = new[] {
             _spritesheet.GetSprite(112, new(3, 3)),
        };
    }

    public void Dispose()
    {
        _resources.Unload<Texture>("sprite.stumpy-tileset");
    }

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        var energyStatus = _world.GetEntityBuckets()
            .Where(x => x.HasComponent<EnergyManagement>() && x.HasComponent<Sprite>())
            .Select(x => x.GetIndices().Select(i => (x.GetEntity(i), x.GetComponent<EnergyManagement>(i))))
            .SelectMany(x => x)
            .FirstOrDefault();

        if (energyStatus.Item2 == null) return ValueTask.CompletedTask;

        int energy = energyStatus.Item2.Value;
        if (energy <= 0)
        {
            _world.SetComponent(energyStatus.Item1, _treeDeadSprites[0]);
            _world.SetComponent(energyStatus.Item1, new Size(new(48, 48)));
        }
  
        return ValueTask.CompletedTask;
    }
}
