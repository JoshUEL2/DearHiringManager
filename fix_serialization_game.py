import re

path = 'Assets/Scripts/1 Data/GameManager.cs'
with open(path, 'r') as f:
    content = f.read()

if "using UnityEngine.Serialization;" not in content:
    content = content.replace("using UnityEngine;", "using UnityEngine;\nusing UnityEngine.Serialization;")

replacements = {
    r'\[SerializeField\] private SavedGame _currentData;': r'[FormerlySerializedAs("currentData")] [SerializeField] private SavedGame _currentData;',
    r'\[SerializeField\] private bool _isGameOver = false;': r'[FormerlySerializedAs("isGameOver")] [SerializeField] private bool _isGameOver = false;',
    r'\[SerializeField\] private string _gameOverReason = "";': r'[FormerlySerializedAs("gameOverReason")] [SerializeField] private string _gameOverReason = "";',
    r'\[SerializeField\] private RandomEventSO _activeEventPopup = null;': r'[FormerlySerializedAs("activeEventPopup")] [SerializeField] private RandomEventSO _activeEventPopup = null;',
    r'\[SerializeField\] private string _activeNewsFeed': r'[FormerlySerializedAs("activeNewsFeed")] [SerializeField] private string _activeNewsFeed',
    r'\[SerializeField\] private List<JobListingSO> _availableJobOpenings = new List<JobListingSO>\(\);': r'[FormerlySerializedAs("availableJobOpenings")] [SerializeField] private List<JobListingSO> _availableJobOpenings = new List<JobListingSO>();',
    r'\[SerializeField\] private List<FinancialSnapshot> _financialHistory = new List<FinancialSnapshot>\(\);': r'[FormerlySerializedAs("financialHistory")] [SerializeField] private List<FinancialSnapshot> _financialHistory = new List<FinancialSnapshot>();',
    r'\[SerializeField\] private bool _hasHustledThisWeek = false;': r'[FormerlySerializedAs("hasHustledThisWeek")] [SerializeField] private bool _hasHustledThisWeek = false;',
    r'\[SerializeField\] private float _rentDiscountThisWeek = 0.0f;': r'[FormerlySerializedAs("rentDiscountThisWeek")] [SerializeField] private float _rentDiscountThisWeek = 0.0f;',
    r'\[SerializeField\] private float _jobMarketLuckBonus = 0.0f;': r'[FormerlySerializedAs("jobMarketLuckBonus")] [SerializeField] private float _jobMarketLuckBonus = 0.0f;',
    r'\[SerializeField\] private bool _completedRegularTaskThisWeek = false;': r'[FormerlySerializedAs("completedRegularTaskThisWeek")] [SerializeField] private bool _completedRegularTaskThisWeek = false;',
    r'\[SerializeField\] private GameDatabaseSO _gameDatabase;': r'[FormerlySerializedAs("gameDatabase")] [SerializeField] private GameDatabaseSO _gameDatabase;'
}

for k, v in replacements.items():
    content = re.sub(k, v, content)

with open(path, 'w') as f:
    f.write(content)

