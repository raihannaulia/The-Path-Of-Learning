using UnityEngine;
using System.Collections;

public class RingPuzzle : MonoBehaviour
{
    [Header("Setingan")]
    public float rotateSpeed = 5f;

    [Header("Debug (Jangan Diubah Manual)")]
    public float currentZ = 0f;    // Kita catat sudut kita sendiri di sini
    public bool isCorrect = false; // Status benar/salah

    private bool isRotating = false;
    private PuzzleManager manager;

    void Start()
    {
        manager = FindObjectOfType<PuzzleManager>();

        // 1. Pilih sudut acak (90, 180, atau 270) - JANGAN ADA 0
        float[] possibleAngles = { 90f, 180f, 270f };
        float randomStart = possibleAngles[Random.Range(0, possibleAngles.Length)];

        // 2. Terapkan ke Visual (Parent)
        // Kita simpan di variabel currentZ agar kita ingat posisinya
        currentZ = randomStart;

        // Update rotasi asli Unity (X tetap -90, Z sesuai acak)
        transform.localRotation = Quaternion.Euler(-90, 0, currentZ);

        CheckWin(); // Cek di awal
    }

    void OnMouseDown()
    {
        if (!isRotating && !manager.isGameWon)
        {
            StartCoroutine(RotateRing());
        }
    }

    IEnumerator RotateRing()
    {
        isRotating = true;

        // Target kita selanjutnya: Sudut sekarang + 90
        float startAngle = currentZ;
        float targetAngle = currentZ + 90f;

        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * rotateSpeed;
            // Kita gerakkan angka currentZ pelan-pelan
            float animatedAngle = Mathf.Lerp(startAngle, targetAngle, t);

            // Terapkan ke Unity
            transform.localRotation = Quaternion.Euler(-90, 0, animatedAngle);
            yield return null;
        }

        // Pastikan angka bulat sempurna di akhir
        currentZ = targetAngle;

        // KUNCI RAHASIA: Jika sudah 360, kembalikan jadi 0 biar gampang
        if (currentZ >= 360f) currentZ -= 360f;

        // Kunci posisi akhir
        transform.localRotation = Quaternion.Euler(-90, 0, currentZ);

        isRotating = false;

        CheckWin();
        manager.CheckWinCondition();
    }

    void CheckWin()
    {
        // Logika Sederhana: Benar jika currentZ adalah 0 (atau 360)
        // Kita pakai Mathf.Approximately untuk membandingkan angka koma
        bool isZero = Mathf.Abs(currentZ) < 1f;       // Dekat 0?
        bool is360 = Mathf.Abs(currentZ - 360f) < 1f; // Dekat 360?

        if (isZero || is360)
        {
            isCorrect = true;
            Debug.Log(gameObject.name + " BENAR! (Sudut: " + currentZ + ")");
        }
        else
        {
            isCorrect = false;
            Debug.Log(gameObject.name + " SALAH (Sudut: " + currentZ + ")");
        }
    }
}