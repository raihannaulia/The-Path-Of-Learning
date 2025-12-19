using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Daftar Peserta")]
    public RingPuzzle ringLuar;
    public RingPuzzle ringTengah;
    public RingPuzzle ringDalam;

    [Header("Hadiah")]
    public GameObject objectHadiah; // Misal: Peta Hologram, Partikel, atau Teks "WIN"
    public AudioSource suaraMenang;

    [HideInInspector] public bool isGameWon = false;

    void Start()
    {
        // Sembunyikan hadiah di awal
        if (objectHadiah != null) objectHadiah.SetActive(false);
    }

    public void CheckWinCondition()
    {
        // Cek: Apakah Ketiga cincin statusnya TRUE?
        if (ringLuar.isCorrect && ringTengah.isCorrect && ringDalam.isCorrect)
        {
            WinGame();
        }
    }

    void WinGame()
    {
        if (isGameWon) return; // Biar gak jalan 2x
        isGameWon = true;

        Debug.Log("PUZZLE SELESAI! MENANG!");

        // 1. Munculkan Hadiah
        if (objectHadiah != null) objectHadiah.SetActive(true);

        // 2. Mainkan Suara
        if (suaraMenang != null) suaraMenang.Play();
    }
}