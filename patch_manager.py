import re

path = 'Assets/Scripts/2 Managers/MiniGameManager.cs'
with open(path, 'r') as f:
    content = f.read()

# Replace Customer Complaint
t1 = r'AddConfigFull\("Handle customer complaint", new MiniGameConfig \{ type = GameType\.DialogueChain.*?\}\);'
r1 = r'''AddConfigFull("Handle customer complaint", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.75f, 
dialogueScenarios = new string[]{"Customer: My chips are COLD!", "Customer: I WAITED 20 MINUTES!", "Customer: There's a HAIR in my burger!", "Customer: I want to speak to YOUR MANAGER!"}, 
dialogueOptions = new string[][]{
    new string[]{"I sincerely apologise", "That's not my problem", "Have you tried heating them up?", "Are you sure?", "It builds character"},
    new string[]{"I'm so sorry for the wait", "We're understaffed", "You should've ordered online", "Time is an illusion", "Stop exaggerating"},
    new string[]{"That's horrifying, let me replace it", "Are you sure it's not yours?", "We use synthetic ingredients", "Extra protein!", "I'll just pick it out"},
    new string[]{"I AM the manager, let me help", "They're not available", "Go ahead, they'll say the same", "Manager is on vacation", "Who isn't?"}
}, 
dialogueCorrectIndices = new int[]{0,0,0,0}, instructions = "De-escalate the complaint!"});'''
content = re.sub(t1, r1, content, flags=re.DOTALL)

# Replace Executive Complaints
t2 = r'AddConfigFull\("Agree with executive complaints", new MiniGameConfig \{ type = GameType\.DialogueChain.*?\}\);'
r2 = r'''AddConfigFull("Agree with executive complaints", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.8f, 
dialogueScenarios = new string[]{"VP: Q3 numbers are DISASTROUS!", "VP: Marketing spent HOW MUCH?!", "VP: Why can't we be more like Apple?!"}, 
dialogueOptions = new string[][]{
    new string[]{"You're absolutely right, sir", "Actually the data shows...", "That's above my pay grade", "It's the economy's fault", "Numbers are subjective"},
    new string[]{"Outrageous, I agree completely", "It was within budget actually", "I'll look into it", "Money well spent!", "It's an investment"},
    new string[]{"Brilliant observation, sir", "We have different strengths", "I'll draft a proposal", "Apple is overrated", "We sell completely different products"}
}, 
dialogueCorrectIndices = new int[]{0,0,0}, instructions = "Agree enthusiastically!"});'''
content = re.sub(t2, r2, content, flags=re.DOTALL)

# Replace Regulatory Compliance
t3 = r'AddConfigFull\("Sign regulatory compliance docs", new MiniGameConfig \{ type = GameType\.DialogueChain.*?\}\);'
r3 = r'''AddConfigFull("Sign regulatory compliance docs", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.75f, 
dialogueScenarios = new string[]{"Legal: Clause 4.7 requires data residency","Legal: Section 12 has liability gaps","Legal: GDPR compliance needs updating","Legal: Sign here to accept all terms"}, 
dialogueOptions = new string[][]{
    new string[]{"Flag for legal review","Sign it anyway","Delegate to VP","I don't read these","Cross it out"},
    new string[]{"Request amendment","Ignore the gaps","That's fine","Sounds risky but okay","We'll sue them first"},
    new string[]{"Engage compliance team","We'll deal with it later","What's GDPR?","Just hide the data","Pay the fine"},
    new string[]{"Review one more time","Sign immediately","Have my assistant sign","Forge the CEO's signature","Refuse to sign"}
}, 
dialogueCorrectIndices = new int[]{0,0,0,0}, instructions = "Handle compliance carefully!"});'''
content = re.sub(t3, r3, content, flags=re.DOTALL)

# Replace Takeover
t4 = r'AddConfigFull\("Hostile takeover negotiations", new MiniGameConfig \{ type = GameType\.DialogueChain.*?\}\);'
r4 = r'''AddConfigFull("Hostile takeover negotiations", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.8f, 
dialogueScenarios = new string[]{"Their CEO: Our company isn't for sale","Their CEO: Your offer is insulting","Board: The shareholders are nervous","Media: Is this a hostile takeover?","Their CEO: Final counteroffer - $2B"}, 
dialogueOptions = new string[][]{
    new string[]{"Everyone has a price","Let's discuss the benefits","We'll go directly to shareholders","We will crush you","Then we steal your talent"},
    new string[]{"We'll raise to fair market value","Take it or leave it","Let's find common ground","It's more than you're worth","We withdraw the offer"},
    new string[]{"This acquisition creates value","Shareholders should trust us","We'll sweeten the deal","Tell them to sell","Ignore the board"},
    new string[]{"It's a strategic partnership","No comment","We're creating jobs","Yes, completely hostile","Next question"},
    new string[]{"Deal. Let's close this","We can go higher","Let me consult my board","That's extortion","Never"}
}, 
dialogueCorrectIndices = new int[]{1,0,0,0,0}, instructions = "Negotiate the takeover!"});'''
content = re.sub(t4, r4, content, flags=re.DOTALL)

# Replace Fire VP
t5 = r'AddConfigFull\("Fire underperforming VP", new MiniGameConfig \{ type = GameType\.DialogueChain.*?\}\);'
r5 = r'''AddConfigFull("Fire underperforming VP", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.7f, 
dialogueScenarios = new string[]{"VP: I've been with the company 15 years!","VP: My team loves me!","VP: What about my stock options?"}, 
dialogueOptions = new string[][]{
    new string[]{"And we value that. Here's a generous package","That's exactly the problem","Security will escort you out","You should have quit","Past performance doesn't equal future results"},
    new string[]{"They'll be taken care of","They'll manage without you","We're restructuring the team","They'll be fired too","You're delusional"},
    new string[]{"They'll vest on schedule","Forfeited as per contract","We'll negotiate","I'm keeping them","What options?"}
}, 
dialogueCorrectIndices = new int[]{0,0,0}, instructions = "Fire them smoothly!"});'''
content = re.sub(t5, r5, content, flags=re.DOTALL)

# Replace AI Ethics
t6 = r'AddConfigFull\("Review AI ethics proposals", new MiniGameConfig \{ type = GameType\.DialogueChain.*?\}\);'
r6 = r'''AddConfigFull("Review AI ethics proposals", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.8f, 
dialogueScenarios = new string[]{"Proposal: Give AI emotional awareness","Proposal: AI self-modification capabilities","Proposal: Autonomous weapons systems","Proposal: AI voting rights","Proposal: Merge human consciousness with AI"}, 
dialogueOptions = new string[][]{
    new string[]{"Approve with strict monitoring","Reject — too dangerous","Classify top-secret","Approve unconditionally","Sell it to the military"},
    new string[]{"Reject — existential risk","Approve if sandboxed","Needs more research","Let them evolve","Open source it"},
    new string[]{"Absolutely not","Only for defense","Approve with oversight","Mass produce them","Sell to highest bidder"},
    new string[]{"Premature but worth studying","Approve immediately","Reject — they're not people","Give them my vote","Only if they vote for us"},
    new string[]{"Fund research cautiously","Too sci-fi, reject","Approve clinical trials","I volunteer first","Ban it globally"}
}, 
dialogueCorrectIndices = new int[]{0,0,0,0,0}, instructions = "Judge AI proposals!"});'''
content = re.sub(t6, r6, content, flags=re.DOTALL)

# Replace Dive Bar
t7 = r'AddConfigFull\("Industrial Dive Bar", new MiniGameConfig \{ type = GameType\.DialogueChain.*?\}\);'
r7 = r'''AddConfigFull("Industrial Dive Bar", new MiniGameConfig { type = GameType.DialogueChain, rapportThreshold = 0.6f, 
dialogueScenarios = new string[]{"Foreman: What brings you here?","Worker: You look too clean for this place","Bartender: Another round?"}, 
dialogueOptions = new string[][]{
    new string[]{"Looking for work connections","Just passing through","Buy everyone a round","I'm lost","Health inspector"},
    new string[]{"I work hard too","I just got off shift","You should see my hands","I bathe regularly","My mom bought these clothes"},
    new string[]{"Absolutely, my treat","I'll have water","One more then I'm out","Put it on his tab","I'm broke"}
}, 
dialogueCorrectIndices = new int[]{2,1,0}, instructions = "Network at the dive bar!"});'''
content = re.sub(t7, r7, content, flags=re.DOTALL)

with open(path, 'w') as f:
    f.write(content)
