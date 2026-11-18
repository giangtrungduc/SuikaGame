using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Playing,
    UsingPowerUp,
    Paused,
    GameOver
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private DropController dropController;

    public GameState CurrentState { get; private set; } = GameState.Playing;

    // Gửi ảnh chụp màn hình (Texture2D) sang cho UI Game Over hiển thị
    public event Action<Texture2D> OnGameOverWithSnapshot;
    public event Action<GameState> OnGameStateChanged;

    public void SetGameState(GameState newState)
    {
        if (CurrentState == newState) return;
        CurrentState = newState;

        // Chỉ cho phép kéo thả bóng khi ở trạng thái Playing
        if (dropController != null)
        {
            dropController.enabled = (newState == GameState.Playing);
        }

        Time.timeScale = (newState == GameState.Paused) ? 0f : 1f;
        OnGameStateChanged?.Invoke(newState);
    }

    public void TriggerGameOver()
    {
        if (CurrentState == GameState.GameOver) return;

        SetGameState(GameState.GameOver);
        StartCoroutine(CaptureSnapshotAndEndGameRoutine());
    }

    private IEnumerator CaptureSnapshotAndEndGameRoutine()
    {
        // Đợi kết thúc frame hiện tại để chụp đúng khoảnh khắc trước khi Pop-up hiện lên
        yield return new WaitForEndOfFrame();

        Texture2D snapshot = ScreenCapture.CaptureScreenshotAsTexture();

        // Đóng băng vật lý toàn bộ các viên thạch trong ly
        Time.timeScale = 0f;

        // Gửi ảnh chụp cho UI để mở Pop-up Game Over
        OnGameOverWithSnapshot?.Invoke(snapshot);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}