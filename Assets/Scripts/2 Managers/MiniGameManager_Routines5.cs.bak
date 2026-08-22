using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MiniGameManager : MonoBehaviour
{
    private void SetupIpHack()
    {
        timer = currentConfig.timeLimit > 0 ? currentConfig.timeLimit : 30f;
        ipHackCurrentProgress = 0;
        ipHackShuffleTimer = 5f;

        // Generate 4 unique target numbers (10-99)
        ipHackTargets.Clear();
        while (ipHackTargets.Count < 4)
        {
            int rnd = Random.Range(10, 100);
            if (!ipHackTargets.Contains(rnd))
            {
                ipHackTargets.Add(rnd);
            }
        }

        // Update Top Labels
        if (ipHackContainer != null)
        {
            for (int i = 0; i < 4; i++)
            {
                if (ipHackTargetLabels[i] != null)
                {
                    ipHackTargetLabels[i].text = ipHackTargets[i].ToString("D2");
                    ipHackTargetLabels[i].style.color = new StyleColor(new Color(0.93f, 0.26f, 0.26f)); // Red
                }
            }
        }

        // Initialize Grid Buttons
        if (ipHackGridContainer != null)
        {
            ipHackGridContainer.Clear();
            ipHackGridButtons.Clear();

            for (int i = 0; i < 50; i++)
            {
                Button btn = new Button();
                btn.style.width = new Length(18, LengthUnit.Percent); // 5 cols? No, 10x5 grid means 10 per row.
                btn.style.width = new Length(9, LengthUnit.Percent); // 10% per item, accounting for margin.
                btn.style.height = 45;
                btn.style.marginTop = 2;
                btn.style.marginBottom = 2;
                btn.style.marginLeft = 2;
                btn.style.marginRight = 2;
                btn.style.fontSize = 28;
                btn.style.backgroundColor = new StyleColor(Color.clear);
                btn.style.color = new StyleColor(new Color(0.8f, 0.8f, 0.8f));
                btn.style.unityFontStyleAndWeight = FontStyle.Bold;
                btn.style.borderTopWidth = 0;
                btn.style.borderBottomWidth = 0;
                btn.style.borderLeftWidth = 0;
                btn.style.borderRightWidth = 0;

                int index = i;
                btn.clicked += () => OnIpHackNumberClicked(btn);
                
                ipHackGridButtons.Add(btn);
                ipHackGridContainer.Add(btn);
            }
        }

        ShuffleIpHackGrid();
        activeGameRoutine = StartCoroutine(IpHackRoutine());
    }

    private void ShuffleIpHackGrid()
    {
        if (ipHackGridButtons.Count == 0) return;

        List<int> availableNumbers = new List<int>();

        // Add remaining targets to guarantee they exist on the board
        for (int i = ipHackCurrentProgress; i < 4; i++)
        {
            availableNumbers.Add(ipHackTargets[i]);
        }

        // Fill the rest with random numbers
        while (availableNumbers.Count < 50)
        {
            int rnd = Random.Range(10, 100);
            
            // We shouldn't generate remaining targets to avoid duplicates, 
            // but it's fine if random numbers match ALREADY FOUND targets.
            bool isRemainingTarget = false;
            for (int i = ipHackCurrentProgress; i < 4; i++)
            {
                if (rnd == ipHackTargets[i])
                {
                    isRemainingTarget = true;
                    break;
                }
            }

            if (!isRemainingTarget)
            {
                availableNumbers.Add(rnd);
            }
        }

        // Shuffle the list
        for (int i = 0; i < availableNumbers.Count; i++)
        {
            int r = Random.Range(i, availableNumbers.Count);
            int temp = availableNumbers[i];
            availableNumbers[i] = availableNumbers[r];
            availableNumbers[r] = temp;
        }

        // Assign to buttons
        for (int i = 0; i < 50; i++)
        {
            Button btn = ipHackGridButtons[i];
            btn.text = availableNumbers[i].ToString("D2");
            btn.userData = availableNumbers[i]; // Store integer in userData for easy access
            btn.enabledSelf = true;
            btn.style.backgroundColor = new StyleColor(Color.clear);
            btn.style.color = new StyleColor(new Color(0.8f, 0.8f, 0.8f));
        }
    }

    private void OnIpHackNumberClicked(Button btn)
    {
        if (!isPlaying || btn.userData == null) return;

        int val = (int)btn.userData;

        if (val == ipHackTargets[ipHackCurrentProgress])
        {
            // Correct!
            if (ipHackTargetLabels[ipHackCurrentProgress] != null)
            {
                ipHackTargetLabels[ipHackCurrentProgress].style.color = new StyleColor(new Color(0.06f, 0.72f, 0.5f)); // Green
            }
            
            ipHackCurrentProgress++;
            btn.style.color = new StyleColor(new Color(0.06f, 0.72f, 0.5f)); // Green text temporarily
            btn.enabledSelf = false;

            if (ipHackCurrentProgress >= 4)
            {
                EndGame(true);
            }
        }
        else
        {
            // Wrong! Penalty
            timer -= 5f;
            btn.style.backgroundColor = new StyleColor(new Color(0.93f, 0.26f, 0.26f, 0.4f)); // Red background flash
            btn.style.color = new StyleColor(Color.white);
            btn.enabledSelf = false;
            
            // Time penalty might end game immediately in routine next frame
        }
    }

    private IEnumerator IpHackRoutine()
    {
        isPlaying = true;
        
        while (isPlaying)
        {
            timer -= Time.deltaTime;
            ipHackShuffleTimer -= Time.deltaTime;

            if (ipHackTimerText != null)
            {
                // Format as 00:30:00 (mm:ss:ms) for GTA feel
                int mins = Mathf.FloorToInt(Mathf.Max(0, timer) / 60F);
                int secs = Mathf.FloorToInt(Mathf.Max(0, timer) - mins * 60);
                int ms = Mathf.FloorToInt((Mathf.Max(0, timer) * 1000) % 1000);
                ipHackTimerText.text = string.Format("{0:00}:{1:00}:{2:000}", mins, secs, ms);
            }

            if (timer <= 0)
            {
                isPlaying = false;
                AbortGame();
            }
            else if (ipHackShuffleTimer <= 0)
            {
                ipHackShuffleTimer = 5f;
                ShuffleIpHackGrid();
            }

            yield return null;
        }
    }
}
