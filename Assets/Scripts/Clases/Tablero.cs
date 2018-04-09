using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Tablero
{
    public Tile[,] Matriz { get; set; }

    public Tablero()
    {
        Matriz = new Tile[GameManager.ALTO, GameManager.ANCHO];
        ColocaTierra();
    }

    //Método que crea la matriz de tiles (tierra por defecto)
    public void ColocaTierra()
    {
        //Se rellenan todas las casillas con tierra al inicio
        for (int y = 0; y < GameManager.ALTO; y++)
        {
            for (int x = 0; x < GameManager.ANCHO; x++)
            {
                Matriz[y, x] = new Tile(new Pos(x, y));

            }
        }
    }

    //Se le llama desde GM cuando se ha hecho click en un tile en el estado COLOCACADAVER
    public void ColocaCadaver(int x, int y)
    {
        Matriz[y, x].Cadaver = true;

        if (y + 1 < GameManager.ALTO)
        {
            Matriz[y + 1, x].Sangre = true;
            GameManager.instance.ColocaSpriteSangre(x, y+1);
        }

        if (y - 1 >= 0)
        {
            Matriz[y - 1, x].Sangre = true;
            GameManager.instance.ColocaSpriteSangre(x, y-1);
        }

        if (x + 1 < GameManager.ANCHO)
        {
            Matriz[y, x + 1].Sangre = true;
            GameManager.instance.ColocaSpriteSangre(x + 1, y);
        }
        if (x - 1 >= 0)
        {
            Matriz[y, x - 1].Sangre = true;
            GameManager.instance.ColocaSpriteSangre(x - 1, y);
        }

        Pos posArma;
        do
        {
            Random rnd = new Random();

            int random = rnd.Next(0, 8);

            switch (random)
            {

                case 0: //NORTE
                    posArma = new Pos(x, y - 2);
                    break;

                case 1: //NORESTE
                    posArma = new Pos( x + 1,  y - 1);
                    break;

                case 2: //ESTE
                    posArma = new Pos( x + 2,  y);
                    break;

                case 3: //SURESTE
                    posArma = new Pos( x + 1,  y + 1);
                    break;

                case 4: //SUR
                    posArma = new Pos( x,  y + 2);
                    break;

                case 5://SUROESTE
                    posArma = new Pos( x - 1,  y + 1);
                    break;

                case 6://OESTE
                    posArma = new Pos( x - 2,  y);
                    break;

                case 7://NOROESTE
                    posArma = new Pos( x - 1,  y - 1);
                    break;

                default:
                    posArma = new Pos(0,0);
                    break;

            }

        } while (DentroDelTablero(posArma));

        Matriz[posArma.y, posArma.x].Arma = true;
        GameManager.instance.ColocaSpriteArma(posArma.x,posArma.y);

    }

    bool DentroDelTablero(Pos pos)
    {

        if (!(pos.y < GameManager.ALTO))
            return true;

        if (!(pos.y >= 0))
            return true;

        if (!(pos.x < GameManager.ANCHO))
            return true;

        if (!(pos.x >= 0))
            return true;

        return false;
    }


    //Se le llama desde GM cuando se ha hecho click en un tile en el estado COLOCAAGUJERO
    public void ColocaAgujero(Pos pos)
    {
        Matriz[pos.y, pos.x].Agujero = true;

        ColocaBarro(pos.x, pos.y);

    }

    //Método que establece el tipo de terreno según sea barro normal o barro con sangre
    public void ColocaBarro(int x, int y)
    {
        if (y + 1 < GameManager.ALTO && !Matriz[y + 1, x].Agujero)
        {
            Matriz[y + 1, x].Barro = true;
            GameManager.instance.ColocaSpriteBarro(x, y + 1);
        }

        if (y - 1 >= 0 && !Matriz[y - 1, x].Agujero)
        {
            Matriz[y - 1, x].Barro = true;
            GameManager.instance.ColocaSpriteBarro(x, y - 1);
        }

        if (x + 1 < GameManager.ANCHO && !Matriz[y, x + 1].Agujero)
        {
            Matriz[y , x +1].Barro = true;
            GameManager.instance.ColocaSpriteBarro(x+1, y);
        }

        if (x - 1 >= 0 && !Matriz[y, x - 1].Agujero)
        {
            Matriz[y, x-1].Barro = true;
            GameManager.instance.ColocaSpriteBarro(x-1, y);

        }


    }


}


