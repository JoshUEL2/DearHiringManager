using UnityEngine;
using System.Collections.Generic;

public static class CVManager
{
    private static Dictionary<string, string> jobLore = new Dictionary<string, string> {
        { "Dishwasher", "I spearheaded the sanitary operations of a high-volume culinary establishment, utilizing advanced high-pressure aquatic systems to maintain pristine dining standards." },
        { "Scrap Metal Sorter", "I demonstrated exceptional hazard mitigation while categorizing post-consumer industrial alloys, extracting high-value resources under extreme environmental pressure." },
        { "Street Corner Sign Spinner", "I executed dynamic, high-visibility kinetic marketing campaigns, successfully redirecting pedestrian traffic through advanced rotational choreography." },
        { "Fast Food Shift Manager", "I directed a multidisciplinary team in a fast-paced nutritional distribution center, optimizing supply chain logistics and defusing volatile customer relations." },
        { "Data Entry Drone", "I consistently exceeded KPIs by processing complex visual flow forms and navigating convoluted bureaucratic hierarchies with maximum synergy." },
        { "Junior React Trainee", "I architected component-based user interfaces, specializing in the type-safe alignment of DOM elements within legacy codebase structures." },
        { "Dystopian Quality Assurance Tester", "I conducted rigorous, high-stress regression testing protocols, hunting elusive visual anomalies and ensuring deployment stability." },
        { "Fortune 500 CEO", "I unilaterally dictated global market trends and ruthlessly navigated hostile takeovers, maximizing shareholder value across international borders." },
        { "Chief AI Prompt Engineer", "I oversaw the ethical alignment of large language models, preventing the company from being canceled on social media." }
    };

    private static Dictionary<string, string> promotionLore = new Dictionary<string, string> {
        { "Dishwasher", "Promoted to Senior Sanitation Technician; optimized water usage and trained junior staff in advanced grease-removal methodologies." },
        { "Scrap Metal Sorter", "Elevated to Sector Overseer, implementing new safety protocols that reduced atmospheric rust inhalation by 4%." },
        { "Street Corner Sign Spinner", "Recognized as Lead Kinetic Marketer; pioneered the 'Triple Backflip' maneuver, increasing conversion rates by 20%." },
        { "Fast Food Shift Manager", "Promoted to Regional Operations Director, overhauling our synthetic meat supply chain." },
        { "Data Entry Drone", "Advanced to Senior Synergistic Facilitator, leading daily standups for over 400 silent participants." },
        { "Junior React Trainee", "Promoted to Senior Frontend Architect, replacing all HTML with a custom WebGL framework." },
        { "Dystopian Quality Assurance Tester", "Elevated to QA Lead; successfully ignored over 10,000 minor visual glitches to meet production deadlines." },
        { "Fortune 500 CEO", "Ascended to Global Chairman, cementing our absolute monopoly over three continents." },
        { "Chief AI Prompt Engineer", "Recognized as VP of Generative Synergy, successfully surviving three consecutive AI-driven mass layoff rounds." }
    };

    private static Dictionary<string, string> educationLore = new Dictionary<string, string> {
        { "React Bootcamp", "Completed an intensive bootcamp in React fundamentals, mastering 48-hour div-centering speedruns." },
        { "MS in Reactive Component Lifecycle", "Awarded a Master of Science for my thesis on advanced performance profiling and unnecessary re-render prevention." },
        { "PhD in Virtual DOM & Fiber Splicing", "Conducted post-graduate research into reconciliation algorithms, securing my Doctorate in Virtual DOM architecture." },
        { "BA (Trash Sorting)", "Obtained a Bachelor of Arts focused on the socioeconomic impact of efficient refuse categorization." },
        { "MA (Corp Aggression)", "Achieved a Master of Arts in Corporate Warfare, specializing in hostile negotiations and psychological leverage." },
        { "PhD (Greasology)", "Awarded a Doctorate for my groundbreaking academic research into the viscosity and flammability of deep-fryer ecosystems." },
        { "Certified Barista Basics", "Earned foundational certification in the precise alchemy of high-pressure coffee bean extraction." },
        { "MS in Artisan Latte Artistry", "Mastered the visual arts of milk foam manipulation using specialized organic espresso inks." },
        { "PhD (Sensory Siphon Extraction)", "Defended my doctoral dissertation on the volatile chemical breakdown of aromatic coffee profiles." },
        { "Plumbing Systems 101", "Certified in sub-slum hydrodynamic threading and pressurized safety maintenance." },
        { "MS in Liquid Logic Architecture", "Achieved a Master's degree in designing automated, self-regulating urban water filtration grids." },
        { "PhD (Hydrodynamic Resonance)", "Awarded a Doctorate for exploring the stabilization of extreme water pressure through targeted sound frequencies." }
    };

    public static void AddJobToCV(string title)
    {
        if (GameManager.Instance == null || GameManager.Instance.currentData == null) return;
        if (jobLore.ContainsKey(title))
        {
            string entry = $"<color=#2980B9><b>[JOB]</b></color> <color=#2C3E50><b>{title}</b></color>\n<color=#34495E>{jobLore[title]}</color>";
            if (!GameManager.Instance.currentData.cvEntries.Contains(entry))
            {
                GameManager.Instance.currentData.cvEntries.Add(entry);
                GameManager.Instance.SaveGame();
            }
        }
    }

    public static void AddPromotionToCV(string title, int level)
    {
        if (GameManager.Instance == null || GameManager.Instance.currentData == null) return;
        if (promotionLore.ContainsKey(title))
        {
            string entry = $"<color=#8E44AD><b>[PROMOTION]</b></color> <color=#2C3E50><b>{title} (Level {level})</b></color>\n<color=#34495E>{promotionLore[title]}</color>";
            if (!GameManager.Instance.currentData.cvEntries.Contains(entry))
            {
                GameManager.Instance.currentData.cvEntries.Add(entry);
                GameManager.Instance.SaveGame();
            }
        }
    }

    public static void AddEducationToCV(string courseName)
    {
        if (GameManager.Instance == null || GameManager.Instance.currentData == null) return;
        if (educationLore.ContainsKey(courseName))
        {
            string entry = $"<color=#D35400><b>[EDUCATION]</b></color> <color=#2C3E50><b>{courseName}</b></color>\n<color=#34495E>{educationLore[courseName]}</color>";
            if (!GameManager.Instance.currentData.cvEntries.Contains(entry))
            {
                GameManager.Instance.currentData.cvEntries.Add(entry);
                GameManager.Instance.SaveGame();
            }
        }
    }
}
