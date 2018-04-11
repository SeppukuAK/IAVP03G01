using System;

/// <summary>
/// Guarda toda la representación lógica de la matriz de Tiles
/// Inicializa la matriz, coloca el cadaver y los agujeros
/// </summary>
public class Tablero
{
    //Representación del tablero
    public Tile[,] Matriz { get; set; }

    /// <summary>
    /// Inicializa la matriz a Tiles genéricos
    /// </summary>
    public Tablero()
    {
        Matriz = new Tile[GameManager.ALTO, GameManager.ANCHO];

        //Se inicializan todas las casillas
        for (int y = 0; y < GameManager.ALTO; y++)
        {
            for (int x = 0; x < GameManager.ANCHO; x++)
            {
                Matriz[y, x] = new Tile(new Pos(x, y));

            }
        }
    }

    /// <summary>
    /// Guarda el cadaver, arma y sangre en la representación lógica de Tiles e informa al GameManager de que actualice los Sprites
    /// Se le llama desde GM cuando se ha hecho click en un tile en el estado COLOCACADAVER
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void ColocaCadaver(int x, int y)
    {
        //Coloca cadaver
        Matriz[y, x].Cadaver = true;

        //Coloca sangre
        //Arriba
        if (y + 1 < GameManager.ALTO)
            ColocaSangre(x, y + 1);

        //Abajo
        if (y - 1 >= 0)
            ColocaSangre(x, y - 1);

        //Izquierda
        if (x + 1 < GameManager.ANCHO)
            ColocaSangre(x + 1, y);

        //Derecha
        if (x - 1 >= 0)
            ColocaSangre(x - 1, y);

        //Coloca arma
        Pos posArma;
        do //Tiramos randoms hasta que encontremos una posición que no se salga del tablero
        {
            Random rnd = new Random();

            int random = rnd.Next(0, 8);

            switch (random)
            {

                case 0: //NORTE
                    posArma = new Pos(x, y - 2);
                    break;

                case 1: //NORESTE
                    posArma = new Pos(x + 1, y - 1);
                    break;

                case 2: //ESTE
                    posArma = new Pos(x + 2, y);
                    break;

                case 3: //SURESTE
                    posArma = new Pos(x + 1, y + 1);
                    break;

                case 4: //SUR
                    posArma = new Pos(x, y + 2);
                    break;

                case 5://SUROESTE
                    posArma = new Pos(x - 1, y + 1);
                    break;

                case 6://OESTE
                    posArma = new Pos(x - 2, y);
                    break;

                case 7://NOROESTE
                    posArma = new Pos(x - 1, y - 1);
                    break;

                default:
                    posArma = new Pos(0, 0);
                    break;

            }

        } while (FueraDelTablero(posArma));

        Matriz[posArma.Y, posArma.X].Arma = true;
        GameManager.instance.ColocaSpriteArma(posArma.X, posArma.Y);
    }

    /// <summary>
    /// Método auxiliar de ColocaCadaver() que guarda la sangre en la matriz lógica de Tiles e informa al GameManager de que cambie el sprite
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void ColocaSangre(int x, int y)
    {
        Matriz[y, x].Sangre = true;
        GameManager.instance.ColocaSpriteSangre(x, y);
    }

    /// <summary>
    /// Devuelve si una posición está fuera de los límites del tablero
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool FueraDelTablero(Pos pos)
    {
        if (!(pos.Y < GameManager.ALTO))
            return true;

        if (!(pos.Y >= 0))
            return true;

        if (!(pos.X < GameManager.ANCHO))
            return true;

        if (!(pos.X >= 0))
            return true;

        return false;
    }

    /// <summary>
    /// Guarda el agujero y el barro en la matriz lógica e informa al GameManager de que actualice sprites de Barro
    /// Se le llama desde GM cuando se ha hecho click en un tile en el estado COLOCAAGUJERO
    /// </summary>
    /// <param name="pos"></param>
    public void ColocaAgujero(int x, int y)
    {
        //Coloca agujero
        Matriz[y, x].Agujero = true;

        //Coloca barro
        //Arriba
        if (y + 1 < GameManager.ALTO && !Matriz[y + 1, x].Agujero)
        {
            Matriz[y + 1, x].Barro = true;
            GameManager.instance.ColocaSpriteBarro(x, y + 1);
        }

        //Abajo
        if (y - 1 >= 0 && !Matriz[y - 1, x].Agujero)
        {
            Matriz[y - 1, x].Barro = true;
            GameManager.instance.ColocaSpriteBarro(x, y - 1);
        }

        //Izquierda
        if (x - 1 >= 0 && !Matriz[y, x - 1].Agujero)
        {
            Matriz[y, x - 1].Barro = true;
            GameManager.instance.ColocaSpriteBarro(x - 1, y);

        }

        //Derecha
        if (x + 1 < GameManager.ANCHO && !Matriz[y, x + 1].Agujero)
        {
            Matriz[y, x + 1].Barro = true;
            GameManager.instance.ColocaSpriteBarro(x + 1, y);
        }

    }


}


