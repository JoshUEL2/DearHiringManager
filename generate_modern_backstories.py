import os
import re

modern_satire_backstories = [
    ("Failed Crypto Influencer", "You told your followers to 'buy the dip' right before the coin went to zero. The SEC froze your assets and you're broke."),
    ("AI Prompt Engineer Intern", "You spent 4 years studying liberal arts, only to get an unpaid internship writing 'Make it sound professional' into a chatbot. You quit."),
    ("Uber for Dogs Driver", "You drove a luxury SUV specifically for commuting other people's labradoodles. One of them threw up on the leather. You're unemployed."),
    ("Micro-Influencer Apologist", "You managed PR for influencers with less than 10k followers. One got canceled for a bad tweet, and they threw you under the bus. You're ruined."),
    ("Cloud Kitchen Ghost Chef", "You cooked 14 different virtual restaurant brands out of a single unventilated shipping container. You burnt a single grilled cheese and got fired."),
    ("Corporate Synergizer", "Your job was literally just to say 'let's circle back on that' in Zoom calls. They replaced you with an AI that says 'let's pin a pin in it'. You're jobless."),
    ("Boutique Water Sommelier", "You tasted artisan tap water for a high-end hipster cafe. A customer realized it was just tap water. The cafe folded, and you have nothing."),
    ("Professional Apology Writer", "You wrote PR apologies for canceled celebrities. You accidentally copy-pasted the same apology for a politician and a YouTuber. You're fired."),
    ("SaaS Subscription Auditor", "You were hired to find unused software subscriptions. You accidentally canceled the CEO's Netflix account. You were immediately let go."),
    ("Tiktok Dance Choreographer", "You invented a viral 15-second hand dance. Nobody credited you, a teenager made millions off it, and you're still broke."),
    ("Open-Office Desk Reserver", "Your sole purpose was to run into the office at 5 AM and put jackets on all the good desks for executives. They laid you off via Slack."),
    ("Bespoke Sourdough Starter Sitter", "You babysat fermented yeast jars for traveling millennials. You accidentally baked one of them. The lawsuit took everything."),
    ("Freelance NFT Evaluator", "You spent your life savings buying a JPEG of a bored monkey. The market crashed to literal pennies. You are financially ruined."),
    ("Gluten-Free Water Innovator", "You tried to market 'Gluten-Free Water'. The FDA fined you into oblivion for misleading claims. You're broke."),
    ("Venture Capital Hype Man", "You stood behind startup founders and nodded aggressively during pitches. The funds dried up, and so did your career."),
    ("Substack Newsletter Writer", "You quit your job to write a paid newsletter about 'The Future of Content'. You got exactly 3 subscribers, two of which are your mom. You're broke."),
    ("Wellness Retreat Guru", "You charged thousands for people to stare at a wall and 'manifest'. Someone manifested a refund, the trend died, and you're bankrupt."),
    ("Algorithmic Day Trader", "You built an AI to day-trade meme stocks. It gained sentience just long enough to purposefully bankrupt you. You're ruined."),
    ("Virtual Real Estate Mogul", "You bought a digital mansion in the metaverse for real money. Now the server is shut down and you own a 404 error. You're broke."),
    ("Agile Scrum Master", "You forced software engineers to stand in a circle and pass a talking stick. The engineers revolted. You were chased out of the office."),
    ("Professional Unboxer", "You made a living opening mystery boxes on camera. A box contained live scorpions. You survived, but your channel didn't."),
    ("Corporate Culture Ambassador", "You organized mandatory fun events like 'Mandatory Monday Mimosas'. The employees unionized against your fun. You were terminated."),
    ("Blockchain Evangelist", "You annoyed your family at every Thanksgiving by talking about decentralization. Now you're broke and not invited to Thanksgiving anymore."),
    ("Subscription Box Curator", "You put random artisanal junk in a cardboard box and charged $40 a month for it. People realized it was junk. You went bankrupt."),
    ("TikTok Trend Forecaster", "You predicted the next big dance trend. You were wrong by exactly one day. The internet roasted you into early retirement."),
    ("Remote Work Productivity Monitor", "You secretly watched employees' webcams to make sure they weren't doing laundry. You got caught. HR fired you immediately."),
    ("Boutique Plant Therapist", "You diagnosed sad monsteras in Brooklyn apartments. A fiddle-leaf fig died on your watch, ruining your Yelp rating. You're out of business."),
    ("Artisanal Ice Sculptor", "You carved perfect spheres of ice for $30 cocktails. Global warming melted your freezer truck. You are financially ruined."),
    ("Professional Line Stander", "You stood in line for limited-edition sneakers so rich kids didn't have to. The brand moved to online-only drops. You're unemployed."),
    ("Luxury Avocado Toast Critic", "You traveled the world rating smashed avocados on sourdough. You developed a severe gluten allergy. Your career is over."),
    ("Digital Nomad Guru", "You sold courses on how to work from a beach in Bali. You got terrible sunburn and your laptop was stolen by a monkey. You're back home, broke."),
    ("Corporate Wellness Coach", "You forced executives to do desk-yoga. The CEO pulled a hamstring during a downward dog. You were swiftly fired."),
    ("Hashtag Strategist", "You were paid $100k a year to put #synergy on corporate posts. They figured out ChatGPT could do it for free. You were laid off."),
    ("Viral Apology Video Editor", "You edited videos of influencers sighing and looking down. The trend shifted to dancing apologies. You couldn't adapt. You're jobless."),
    ("Artisan Mayonnaise Sommelier", "You paired different types of mayo with fries. People realized it's just eggs and oil. Your niche restaurant went under."),
    ("Startup Ping Pong Champion", "Your only value to the tech company was beating the rival company at ping pong. You lost the tournament. You lost your job."),
    ("Crypto-Bro Life Coach", "You yelled at guys in finance vests to 'grind harder'. The SEC cracked down on your clients. You're out of a job."),
    ("Professional Slack Reactor", "You were hired solely to add the correct emojis to the CEO's announcements. You accidentally added a clown emoji. You were fired."),
    ("Corporate Buzzword Translator", "You translated 'leveraging synergistic paradigms' into English for the interns. The company pivoted to plain English. You're unemployed.")
]

directory = "/Users/josh/Dear Hiring Manager/Assets/Resources/GameData/"

# Get all backstory files
backstory_files = [f for f in os.listdir(directory) if f.startswith("Backstory_") and f.endswith(".asset")]

for i, filename in enumerate(backstory_files):
    if i >= len(modern_satire_backstories):
        break
    
    title, desc = modern_satire_backstories[i]
    
    filepath = os.path.join(directory, filename)
    with open(filepath, 'r') as f:
        content = f.read()
    
    # Replace the title
    content = re.sub(r'title:.*', f'title: {title}', content)
    
    # Replace the week1Log
    # Since week1Log might be multi-line or missing, we just replace it if it exists, or append it
    if 'week1Log:' in content:
        content = re.sub(r'week1Log:.*', f'week1Log: "{desc}"', content)
    else:
        content += f'\n  week1Log: "{desc}"\n'
        
    # We should also replace the description if it's there
    if 'description:' in content:
        content = re.sub(r'description:.*', f'description: "{desc}"', content)
        
    with open(filepath, 'w') as f:
        f.write(content)

print(f"Updated {len(backstory_files)} backstories with modern satire.")
