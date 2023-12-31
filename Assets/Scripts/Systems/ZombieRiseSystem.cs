﻿using ComponentsAndTags;
using Unity.Burst;
using Unity.Entities;

namespace Systems
{
    [BurstCompile]
    [UpdateAfter(typeof(SpawnZombieSystem))]
    public partial struct ZombieRiseSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var deltaTime = SystemAPI.Time.DeltaTime;
            var ecbSingleton = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>();
            new ZombieRiseJob
            {
                DeltaTime = deltaTime,
                ECB = ecbSingleton.CreateCommandBuffer(state.WorldUnmanaged).AsParallelWriter()
            }.ScheduleParallel();
        }
    }
    
    [BurstCompile]
    public partial struct ZombieRiseJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter ECB;
        [BurstCompile]
        private void Execute(ZombieRiseAspect zombie, [EntityInQueryIndex]int sortKey)
        {
            zombie.Rise(DeltaTime);
            if(!zombie.IsAboveGround) return; // if not above ground, do nothing (return)
            zombie.SetAtGroundLevel(); // if above ground, set zombie at ground level
            ECB.RemoveComponent<ZombieRiseRate>(sortKey, zombie.Entity);
            ECB.SetComponentEnabled<ZombieWalkProperties>(sortKey, zombie.Entity, true);
        }
    }
}