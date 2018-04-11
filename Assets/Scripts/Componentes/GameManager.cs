using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum EstadoEscena { COLOCACADAVER, COLOCAAGUJERO, PAUSA, PLAY };

public class GameManager : MonoBehaviour
{
    //Singleton
    public static GameManager instance;

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

    public Button ButtonComienzaBusqueda;
    public Button ButtonPausaBusqueda;
    public Button ButtonReiniciaBusqueda;

    public Image IconoArma;

    public Text TextoMensaje;

    //--------ATRIBUTOS UNITY--------

    //CONSTANTES
    public const int ANCHO = 10;
    public const int ALTO = 5;
    public const float DISTANCIA = 0.64f;

    private Tablero tablero;
    private Agente agente;
    public Pos PosCasa { get; set; }
    public EstadoEscena Estado { get; set; }

    private GameObject agenteGO;
    private GameObject armaGO;
    private GameObject cadaverGO;
    private GameObject[,] MatrizTiles { get; set; }//matriz de GO tiles

    //--------ATRIBUTOS--------------

    void Awake()
    {
        //GameManager es Singleton
        instance = this;
    }

    //----------INICIALIZACION--------------
    void Start()
    {
        //Inicializar GUI
        IconoArma.enabled = false;

        ButtonComienzaBusqueda.gameObject.SetActive(false);
        ButtonPausaBusqueda.gameObject.SetActive(false);
        ButtonReiniciaBusqueda.gameObject.SetActive(false);


        Estado = EstadoEscena.COLOCACADAVER;
        TextoMensaje.text = "Coloca el barco destruido";

        MatrizTiles = new GameObject[ALTO, ANCHO];
        tablero = new Tablero();

        ColocaTablero();
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

        PosCasa = new Pos(Random.Range(0, ANCHO), Random.Range(0, ALTO));
        GameObject casa = Instantiate(casaPrefab, new Vector3(PosCasa.X * DISTANCIA, -PosCasa.Y * DISTANCIA, 0), Quaternion.identity);
    }

    //----------INICIALIZACION--------------



    //-------Se le llama al colocar el cadaver-------------------

    //Se le llama cuando el usuario hace click desde TileView
    public void ColocaCadaver(Pos pos)
    {
        tablero.ColocaCadaver(pos.X, pos.Y);

        cadaverGO = Instantiate(cadaverPrefab, new Vector3(pos.X * DISTANCIA, -pos.Y * DISTANCIA, 0), Quaternion.identity);


        //Cambiamos el estado
        Estado = EstadoEscena.COLOCAAGUJERO;
        TextoMensaje.text = "Coloca los obstáculos y pulsa el botón Comienza Busqueda cuando quieras que el agente comience su busqueda";

        ButtonComienzaBusqueda.gameObject.SetActive(true);
    }

    //------Se le llama desde Tablero.ColocaCadaver()------------

    public void ColocaSpriteSangre(int x, int y)
    {
        MatrizTiles[y, x].GetComponent<SpriteRenderer>().sprite = spriteSangre;
    }

    public void ColocaSpriteArma(int x, int y)
    {
        armaGO = Instantiate(armaPrefab, new Vector3(x * DISTANCIA, -y * DISTANCIA, 0), Quaternion.identity);
    }

    //------Se le llama desde Tablero.ColocaCadaver()----------

    //-------Se le llama al colocar el cadaver-------------------//


    //-------Se le llama al colocar el agujero-------------------//

    //Se le llama cuando el usuario hace click desde TileView
    public void ColocaAgujero(Pos pos)
    {
        tablero.ColocaAgujero(pos.X,pos.Y); //coloca el agujero en la matriz logica

        //Aplicamos el sprite
        MatrizTiles[pos.Y, pos.X].GetComponent<SpriteRenderer>().sprite = spriteAgujero;
    }

    public void ColocaSpriteBarro(int x, int y) 
    {
        if (tablero.Matriz[y, x].Sangre)
            MatrizTiles[y, x].GetComponent<SpriteRenderer>().sprite = spriteSangreBarro;

        else
            MatrizTiles[y, x].GetComponent<SpriteRenderer>().sprite = spriteBarro;

    }

    //-------Se le llama al colocar el agujero-------------------//

    //Métodos de comprobacion

    public bool HayCasa(Pos pos)
    {
        return (pos.Equals(PosCasa));
    }


    //-------CALLBACKS DE BOTONES-------------------//

    //Boton ComienzaBosqueda
    public void IniciaBusqueda()
    {
        Estado = EstadoEscena.PLAY;
        TextoMensaje.text = "";

        ButtonComienzaBusqueda.gameObject.SetActive(false);
        ButtonPausaBusqueda.gameObject.SetActive(true);
        ButtonReiniciaBusqueda.gameObject.SetActive(true);

        agenteGO = Instantiate(detectivePrefab, new Vector3(PosCasa.X * DISTANCIA, -PosCasa.Y * DISTANCIA, 0), Quaternion.identity);

        agente = new Agente(tablero.Matriz[PosCasa.Y, PosCasa.X]);

        for (int y = 0; y < GameManager.ALTO; y++)
        {
            for (int x = 0; x < GameManager.ANCHO; x++)
            {
                //Casilla
                MatrizTiles[y, x].GetComponent<SpriteRenderer>().color = Color.black;
            }

        }
        MatrizTiles[PosCasa.Y, PosCasa.X].GetComponent<SpriteRenderer>().color = Color.white;

        armaGO.GetComponent<SpriteRenderer>().enabled = false;
        cadaverGO.GetComponent<SpriteRenderer>().enabled = false;

        StartCoroutine("ContinuaBusqueda");
    }

    //Boton Pausa
    public void PausaReanudaBusqueda()
    {
        if (Estado == EstadoEscena.PLAY)
            Estado = EstadoEscena.PAUSA;
        else
        {
            Estado = EstadoEscena.PLAY;
            StartCoroutine("ContinuaBusqueda");
        }

    }
    //Boton Reinicio
    public void ReiniciaBusqueda()
    {
        SceneManager.LoadScene("Practica3");

    }
    //-------CALLBACKS DE BOTONES-------------------//

    private bool haciendoCamino = false;
    IEnumerator ContinuaBusqueda()
    {
        while (!agente.ObjetivoCumplido() && !agente.AgenteMuerto() && Estado == EstadoEscena.PLAY )
        {
            if (!haciendoCamino)
            {
                haciendoCamino = true;
                agente.Avanza();
            }

             yield return new WaitForSeconds(0.016f);
        }

        while(haciendoCamino)
            yield return new WaitForSeconds(0.016f);

        if (agente.ObjetivoCumplido())
            agente.VuelveACasa();

        //HACER ALGO
        else if (agente.AgenteMuerto())
            TextoMensaje.text = "El agente ha fallado en su misión";
        

        yield return null;

    }


    //Necesario para conocimientoAgente
    public Tile GetTile(Pos pos)
    {
        return tablero.Matriz[pos.Y, pos.X];
    }

 
    public void MoverAgente(Stack<Pos> camino)
    {
        if (camino != null)
            StartCoroutine("AvanzaUnPaso", camino);

    }

    IEnumerator AvanzaUnPaso(Stack<Pos> camino)
    {
        Pos pos = new Pos(0,0);
        while (camino.Count > 0)
        {
            pos = camino.Pop();

            agente.Pos = pos;
            agenteGO.transform.position = new Vector3(pos.X * DISTANCIA, -pos.Y * DISTANCIA, 0);

            yield return new WaitForSeconds(0.2f);
        }

        if(agente.CuerpoEncontrado())
            cadaverGO.GetComponent<SpriteRenderer>().enabled = true;

        if (agente.ArmaEncontrada())
        {
            armaGO.GetComponent<SpriteRenderer>().enabled = false;
            IconoArma.enabled = true;
        }

        MatrizTiles[pos.Y, pos.X].GetComponent<SpriteRenderer>().color = Color.white;

        haciendoCamino = false;
        yield return null;
    }

}
