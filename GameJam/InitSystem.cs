using GameJam.Components;
using GameJam.Engine.Components;
using GameJam.Engine.Rendering.Components;
using GameJam.Engine.Resources;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System.Security.Cryptography;

namespace GameJam;

public class InitSystem : IHostedService
{

    public InitSystem(IWindow window, IWorldManager worldManager, ILogger<InitSystem> logger, IResourceManager resources)
    {
        window.Load += () =>
        {
            logger.LogInformation("Init Game");
            var world = worldManager.CreateWorld();
            var spriteSheet = resources.Load<Texture>("sprite.stumpy-tileset");
            logger.LogInformation("Creating Entity");
            var entity = world.CreateEntity();
            world.SetComponent(entity, new Position(new(0, 0)));
            world.SetComponent(entity, new Sprite(spriteSheet, new(0, 0, 1, 1)));
            world.SetComponent(entity, new Size(new(100, 100)));
            world.SetComponent(entity, new Rotation(0));
            world.SetComponent(entity, new SpriteLayer(0, 0));

            var camEntity = world.CreateEntity();
            world.SetComponent(camEntity, new Camera(200));
            world.SetComponent(camEntity, new Position(new(0, 0)));


            //CreateGrid(10, 10);

            
            void CreateGrid(int width, int height)
            {
                foreach (var idx in Enumerable.Range(0, width * height))
                {
                    var x = idx / width;
                    var y = idx % width;
                    var nodeEntity = world.CreateEntity();
                    world.SetComponent(nodeEntity, new Node());
                    world.SetComponent(nodeEntity, new Position(new Vector2D<int>(x, y)));

                    
                    CreateRoot(x, y, nodeEntity);
                }
            }

            void CreateRoot(int x, int y, Entity nodeEntity)
            {
                var rootEntity = world.CreateEntity();
                world.SetComponent(rootEntity, new Position(new Vector2D<int>(x, y)));
                world.SetComponent(rootEntity, new Sprite(spriteSheet, new System.Numerics.Vector4(0,0,1,1)));
                world.SetComponent(rootEntity, new Rotation(0));
                world.SetComponent(rootEntity, new SpriteLayer(0, 0));
            }
        };
    }
    

    public ValueTask Execute(CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}