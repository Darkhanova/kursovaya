using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kursach
{
    class Emitter
    {
        List<Particle> particles = new List<Particle>();
        public float GravitationX = 0;
        public float GravitationY = 1; // пусть гравитация будет силой один пиксель за такт, нам хватит
        public int X; // координата X центра эмиттера, будем ее использовать вместо MousePositionX
        public int Y; // соответствующая координата Y 
        public int Direction = 0; // вектор направления в градусах куда сыпет эмиттер
        public int Spreading = 0; // разброс частиц относительно Direction
        public int speed = 10; // начальная максимальная скорость движения частицы
        public int RadiusMin = 2; // минимальный радиус частицы
        public int RadiusMax = 10; // максимальный радиус частицы
        public int LifeMin = 20; // минимальное время жизни частицы
        public int LifeMax = 20; // максимальное время жизни частицы
        public int ParticlesCount = 1;
        public List<Point> gravityPoints = new List<Point>();
        public List<IImpactPoint> impactPoints = new List<IImpactPoint>();

        public int ParticlesPerTick = 500; // добавил новое поле

        public Color ColorFrom = Color.Pink; // начальный цвет частицы
        public Color ColorTo = Color.FromArgb(0, Color.Black); // конечный цвет частиц

        public virtual void ResetParticle(Particle particle)
        {
            particle.Life = Particle.rand.Next(LifeMin, LifeMax);

            particle.X = X;
            particle.Y = Y;

            var direction = Direction
                + (double)Particle.rand.Next(Spreading)
                - Spreading / 2;

            particle.SpeedX = (float)(Math.Cos(direction / 180 * Math.PI) * speed);
            particle.SpeedY = -(float)(Math.Sin(direction / 180 * Math.PI) * speed);

            particle.Radius = Particle.rand.Next(RadiusMin, RadiusMax);
            
        }

        /* добавил метод */
        public virtual Particle CreateParticle()
        {
            ParticlesCount++;
            var particle = new ParticleColorful();
            particle.FromColor = ColorFrom;
            particle.ToColor = ColorTo;
            
            return particle;
        }

        public void UpdateState()
        {
            int particlesToCreate = ParticlesPerTick; // фиксируем счетчик сколько частиц нам создавать за тик

            foreach (var particle in particles)
            {
                particle.Life -= 1;
                if (particle.Life < 0)
                {
                   
                    if (particlesToCreate > 0)
                    {
                        /* у нас как сброс частицы равносилен созданию частицы */
                        particlesToCreate -= 1; // поэтому уменьшаем счётчик созданных частиц на 1
                        ResetParticle(particle);
                    }
                }
                else
                {
                    // каждая точка по-своему воздействует на вектор скорости
                    foreach (var point in gravityPoints)
                    {
                        float gX = point.X - particle.X;
                        float gY = point.Y - particle.Y;
                        float r2 = gX * gX + gY * gY;
                        float M = 100;

                        particle.SpeedX += (gX) * M / r2;
                        particle.SpeedY += (gY) * M / r2;
                    }

                    // это не трогаем
                    particle.SpeedX += GravitationX;
                    particle.SpeedY += GravitationY;

                    particle.X += particle.SpeedX;
                    particle.Y += particle.SpeedY;
                    
                }
            }

            if(particlesToCreate >= 1)
            {
                particlesToCreate -= 1; 
                var particle = CreateParticle();
                ResetParticle(particle);
                particles.Add(particle);
                
            }
            
        }

        public void Render(Graphics g)
        {
            // не трогаем
            foreach (var particle in particles)
            {
                particle.Draw(g);
                
            }

            foreach (var point in impactPoints) // тут теперь  impactPoints
            {
                
                point.Render(g); // это добавили
                
            }
        }
    }
}
