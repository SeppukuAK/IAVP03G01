using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    //--------ATRIBUTOS--------------
    //Posibles Scripts
    public Pos PosCasa { get; set; }
    public Pos PosCadaver { get; set; }
    public Pos PosArma { get; set; }

    public Estado Estado { get; set; }
    public GameObject[,] MatrizTiles { get; set; }//matriz de GO tiles

    //Macros
    public const int ANCHO = 10;
    public const int ALTO = 5;
    public const float DISTANCIA = 0.70f;

    Tablero tablero;
   
    int numAgujeros;//numero de agujeros que puede colocar el usuario

    //--------ATRIBUTOS--------------

    //--------ATRIBUTOS UNITY--------

    public GameObject tilePrefab;
    public GameObject casaPrefab;
    public GameObject cadaverPrefab;
    public GameObject detectivePrefab;
    public GameObject armaPrefab;

    public Sprite spriteBarro;
    public Sprite spriteAgujero;
    public Sprite spriteSangre;
    public Sprite spriteSangreBarro;

    //--------ATRIBUTOS UNITY--------


    void Start()
    {
        instance = this;

        Estado = Estado.COLOCACADAVER;
        numAgujeros = 3;

        MatrizTiles = new GameObject[ALTO, ANCHO];
        tablero = new Tablero();

        ColocaTablero();

    }

    void Update()
    {

    }

    //Pasa la representación lógica del tablero (matriz) a la representación física (gameobjects)
    void ColocaTablero()
    {
        GameObject GOTablero = new GameObject("Tablero");

        for (int y = 0; y < GameManager.ALTO; y++)
        {
            for (int x = 0; x < GameManager.ANCHO; x++)
            {
                //Creamos gameObject
                MatrizTiles[y, x] = Instantiate(tilePrefab, new Vector3(x * DISTANCIA, -y * DISTANCIA, 0), Quaternion.identity, GOTablero.transform);

                Tile tileAux = tablero.Matriz[y, x];

                //Casilla
                MatrizTiles[y, x].GetComponent<TileView>().ConstruyeCasilla(tileAux);
            }

        }
        ColocaCasa();

    }

    //Se le llama al generar el tablero
    void ColocaCasa()
    {
        PosCasa = new Pos(Random.Range(0, 10), Random.Range(0, 5));
        GameObject casa = Instantiate(casaPrefab, new Vector3(PosCasa.x * DISTANCIA, -PosCasa.y * DISTANCIA, 0), Quaternion.identity);

    }

    //Se le llama cuando el usuario hace click desde TileView
    public void ColocaCadaver(Pos pos)
    {
        //Coloca cadaver
        PosCadaver = pos;
        GameObject cadaver = Instantiate(cadaverPrefab, new Vector3(PosCadaver.x * DISTANCIA, -PosCadaver.y * DISTANCIA, 0), Quaternion.identity);

        //Coloca Sangre
        tablero.ColocaSangre(pos.x, pos.y);
        ColocaSpriteSangre(pos.x, pos.y);

        //Coloca Arma
        ColocaArma();

        //Cambiamos el estado
        Estado = Estado.COLOCAAGUJERO;
    }

    //-------Se le llama al colocar el cadaver-------------------

    void ColocaArma()
    {

        do
        {
            int random = Random.Range(0, 8);

            switch (random)
            {

                case 0: //NORTE
                    PosArma = new Pos(PosCadaver.x, PosCadaver.y - 2);
                    break;

                case 1: //NORESTE
                    PosArma = new Pos(PosCadaver.x + 1, PosCadaver.y - 1);
                    break;

                case 2: //ESTE
                    PosArma = new Pos(PosCadaver.x + 2, PosCadaver.y);
                    break;

                case 3: //SURESTE
                    PosArma = new Pos(PosCadaver.x + 1, PosCadaver.y + 1);
                    break;

                case 4: //SUR
                    PosArma = new Pos(PosCadaver.x, PosCadaver.y + 2);
                    break;

                case 5://SUROESTE
                    PosArma = new Pos(PosCadaver.x - 1, PosCadaver.y + 1);
                    break;

                case 6://OESTE
                    PosArma = new Pos(PosCadaver.x - 2, PosCadaver.y);
                    break;

                case 7://NOROESTE
                    PosArma = new Pos(PosCadaver.x - 1, PosCadaver.y - 1);
                    break;

            }
                
        } while (DentroDelTablero(PosArma));

        GameObject arma = Instantiate(armaPrefab, new Vector3(PosArma.x * DISTANCIA, -PosArma.y * DISTANCIA, 0), Quaternion.identity);
    }

    bool DentroDelTablero(Pos pos) 
    {

        if (!(pos.y < ALTO))
            return true;

        if (!(pos.y >= 0))
            return true;

        if (!(pos.x < ANCHO))
            return true;

        if (!(pos.x >= 0))
            return true;

        return false;
    }

    //-------Se le llama al colocar el cadaver-------------------//


    //Se le llama cuando el usuario hace click desde TileView
    public void ColocaAgujero(Pos pos)
    {
        numAgujeros--; //Se reduce el numero de agujeros a colocar
        tablero.ColocaAgujero(pos); //coloca el agujero en la matriz logica

        //Aplicamos el sprite
        MatrizTiles[pos.y, pos.x].GetComponent<SpriteRenderer>().sprite = spriteAgujero;
        ColocaSpriteBarro(pos.x, pos.y);

        //Cuando el numero de agujeros es 0 pasamos al estado de Pausa, antes de que la IA empiece
        if (numAgujeros == 0)
            Estado = Estado.PAUSA;
    }


    //-------Se le llama al colocar el agujero-------------------

    void ColocaSpriteSangre(int x, int y)
    {
        if (y + 1 < ALTO && tablero.Matriz[y + 1, x].terreno != Terreno.AGUJERO)
            MatrizTiles[y + 1, x].GetComponent<SpriteRenderer>().sprite = spriteSangre;

        if (y - 1 >= 0 && tablero.Matriz[y - 1, x].terreno != Terreno.AGUJERO)
            MatrizTiles[y - 1, x].GetComponent<SpriteRenderer>().sprite = spriteSangre;

        if (x + 1 < ANCHO && tablero.Matriz[y, x + 1].terreno != Terreno.AGUJERO)
            MatrizTiles[y, x + 1].GetComponent<SpriteRenderer>().sprite = spriteSangre;

        if (x - 1 >= 0 && tablero.Matriz[y, x - 1].terreno != Terreno.AGUJERO)
            MatrizTiles[y, x - 1].GetComponent<SpriteRenderer>().sprite = spriteSangre;
    }
    
    void ColocaSpriteBarro(int x, int y) 
    {
        if (y + 1 < ALTO && tablero.Matriz[y + 1, x].terreno != Terreno.AGUJERO)
            DeterminaSangreBarro(MatrizTiles[y + 1, x], tablero.Matriz[y + 1, x]);                 

        if (y - 1 >= 0 && tablero.Matriz[y - 1, x].terreno != Terreno.AGUJERO)      
            DeterminaSangreBarro(MatrizTiles[y - 1, x], tablero.Matriz[y - 1, x]);       

        if (x + 1 < ANCHO && tablero.Matriz[y, x + 1].terreno != Terreno.AGUJERO)        
            DeterminaSangreBarro(MatrizTiles[y, x + 1], tablero.Matriz[y, x + 1]);
     
        if (x - 1 >= 0 && tablero.Matriz[y, x - 1].terreno != Terreno.AGUJERO)       
            DeterminaSangreBarro(MatrizTiles[y, x - 1], tablero.Matriz[y, x - 1]);
        
    }

    void DeterminaSangreBarro(GameObject go, Tile tile)
    {
        if (tile.terreno == Terreno.SANGREBARRO)
            go.GetComponent<SpriteRenderer>().sprite = spriteSangreBarro;

        else
            go.GetComponent<SpriteRenderer>().sprite = spriteBarro;
    }

    //-------Se le llama al colocar el agujero------------------//

    //Métodos de comprobacion

    public bool HayCadaver(Pos pos)
    {
        return (pos.Equals(PosCadaver));
    }

    public bool HayCasa(Pos pos)
    {
        return (pos.Equals(PosCasa));
    }

    public bool HayArma(Pos pos)
    {
        return (pos.Equals(PosArma));
    }
}
