# Spec: Healing Potion

Status: ready-for-agent

## Problem Statement

Health Points only ever go down. Once the dragon starts landing hits, the player has no
way to recover: they can keep attacking or retreat, and nothing they did on the way north
helps them survive a long fight. The Merchant sells gear that makes hits hurt less or the
player hit harder, but nothing that undoes damage already taken.

## Solution

Add a **Healing Potion**: a consumable that restores d8 + 2 Health Points, never above the
player's new **Maximum Health Points**. The player can **find** one by a die roll when
arriving at the south or north path, or **buy** one from the Merchant. They can **drink**
one during combat, using their turn, or after a win or a retreat, before the game ends.

## Confirmed decisions

All decisions below are confirmed by the product owner during grilling (Q1–Q22).

- **Q1 — Name:** the item is the **Healing Potion**. It's the only healing item.
- **Q2 — Maximum Health Points:** a new player value, set to the Health Points rolled at
  character creation. Healing never raises HP above it.
- **Q3 — Heal amount:** d8 + 2 (3–10 HP), capped at Maximum HP.
- **Q4 / Q8 — Finding:** a find roll happens on **arriving** at the south path (before
  the Merchant greeting) and on arriving at the north path (before the dragon appears).
  The path choice menu is not a location and has no roll.
- **Q9 — Find roll:** d20; **16 or higher** finds one potion (25%). A find shows a
  message. A miss shows nothing.
- **Q10 — Find limit:** each path gives **at most one** potion per game. Once a path has
  given one, arriving there no longer rolls. Finding gives at most two potions per game.
- **Q11 / Q16 — Buying:** the Merchant sells a Healing Potion for **30 Gold** (tunable,
  like the other prices).
- **Q17 — Merchant menu:** `1` Magical Armor, `2` Magical Weapon, `3` Healing Potion,
  `4` leave. Leave moves from `3` to `4`.
- **Q18 — Purchase limit:** a potion counts as the visit's one item (merchant decision
  A5), so buying one ends the visit. There's no other limit. Repeat visits can each buy one.
- **Q5 / Q12 — Drinking in combat:** the combat prompt gains a third option, `p` /
  `potion`. Drinking uses the player's turn: they don't attack, the dragon attacks, then
  the prompt returns. The prompt first appears after the first exchange, so the earliest
  drink is after the dragon's first attack.
- **Q13 — Useless drinks:** choosing `p` with 0 potions ("You have no Healing Potions.")
  or at full HP ("You are already at full health.") is refused. No potion is used, no turn
  passes, and the prompt is shown again.
- **Q14 — After combat:** the player is offered a drink only after a **win** or a
  **retreat during combat**, and only if they hold a potion and are below Maximum HP. A
  defeat never gets the offer: potions can't revive.
- **Q15 — After-combat prompt:** `Drink a Healing Potion? (y/n)`, shown with the current
  HP and potion count. It repeats after each drink until the player answers `n`, runs out
  of potions or reaches Maximum HP. End of input counts as `n`. Then the game ends as it
  does now.
- **Q6 — Count:** the player carries a count of Healing Potions, starting at 0, shown in
  the character stats and the combat stats (e.g. `Healing Potions: 2`).
- **Q7 — Languages:** all new text ships in English, Spanish, French and Italian,
  accent-free, like the existing text.

## User Stories

1. As a player, I want to see my Maximum Health Points, so that I know how much healing I can use.
2. As a player, I want my Healing Potion count shown in my character stats and combat stats, so that I know what I'm carrying.
3. As a player, I want a chance to find a Healing Potion when I arrive at the south path, so that exploring pays off.
4. As a player, I want a chance to find a Healing Potion when I arrive at the north path, so that I might face the dragon with a potion even without Gold.
5. As a player, I want to be told when I find a Healing Potion, so that I know my count went up.
6. As a player, I don't want to find more than one potion per path, so that walking back and forth isn't the best way to play.
7. As a player, I want to buy a Healing Potion from the Merchant for 30 Gold, so that I can prepare for the fight.
8. As a player who can't afford a potion, I want the usual can't-afford message, so that I understand why the purchase failed.
9. As a player, I want leave to stay the last option on the Merchant's menu, so that the menu reads naturally.
10. As a player in combat, I want to drink a potion instead of attacking, so that I can survive a little longer.
11. As a player, I want the heal to stop at my Maximum HP, so that potions can't make me stronger than I started.
12. As a player who picks "potion" with none left, or at full health, I want a message and no lost turn, so that a mistake doesn't cost me.
13. As a player who wins or retreats, I want to drink potions before the game ends, so that I can patch myself up.
14. As a player who is defeated, I understand potions can't revive me, so that a defeat stays a defeat.
15. As a Spanish, French or Italian player, I want all potion text in my language, so that the game stays consistent.
16. As a developer, I want every roll, including find rolls, controlled by the test dice, so that the tests stay repeatable.

## Implementation Decisions

- **Player:** gains Maximum Health Points (set when HP is rolled) and a Healing Potion
  count. Drinking a potion rolls d8 + 2 with the player's die, raises HP up to the
  maximum, lowers the count by one and reports the HP gained and the new HP. The stats
  summary shows Maximum HP alongside HP, and the potion count.
- **Find rolls:** the game remembers which paths have already given a potion this game.
  On arriving at a path that hasn't, it rolls a d20 with the game's die. A roll of 16+
  adds one potion and shows the find message.
- **Merchant:** a third offer (Healing Potion, 30 Gold), paid through the existing
  affordability check. Leave becomes `4`, and the menu text, the leave option and the
  invalid-choice text change to match.
- **Combat:** the combat prompt text lists `p`/`potion`. Choosing it with a potion and
  below Maximum HP drinks one, then the dragon attacks, then the prompt returns. It does
  not run the player's attack. The refusal cases print a message and show the prompt
  again. Combat stats show the potion count.
- **After combat:** the dragon-encounter flow offers the drink prompt after a win (before
  the victory message) and after a retreat during combat (after the combat's retreat
  line). A retreat chosen before combat starts, at the dragon-encounter prompt, never
  triggers the offer, because the player can't have lost HP yet.
- **Input running out:** treated as `n` at the after-combat prompt, as "leave" at the
  Merchant, and as before everywhere else.
- **Language data:** new message keys in all four language sections, plus the Healing
  Potion's display name.

## Testing Decisions

- **What makes a good test:** assert on what the player sees (console output) and on
  observable player state (HP, Maximum HP, potion count, Gold). Don't assert on private
  fields or call order. All rolls come from `FixedDie`, so outcomes are deterministic.
- **Existing dice scripts:** the find roll adds a d20 on every south or north arrival, so
  each scripted sequence in `GamePathTests.cs` gets a miss roll (e.g. `1`) inserted where
  the arrival roll happens. There's no test-only switch to turn finding off.
- **Existing merchant tests:** tests that leave with `3` change to `4`.
- **Seam 1, the full game** (scripted console input through the game's start method):
  - A find on arriving at the south path; a find on arriving at the north path.
  - No second find from the same path (south, back, south again: no roll the second time).
  - A miss shows no find message and leaves the count unchanged.
  - Buying a potion deducts 30 Gold, adds one to the count and ends the visit.
  - Trying to buy without enough Gold shows can't-afford and leaves Gold unchanged.
  - The potion count shows in the character stats.
  - After a win and after a retreat during combat, with a potion and below Maximum HP, the
    drink prompt appears. Drinking heals and the prompt repeats. `n` ends the game.
  - No prompt after a defeat, with 0 potions or at full HP.
  - Input running out at the drink prompt ends the game, with no infinite loop. Use the
    existing 5-second timeout pattern.
- **Seam 2, combat** (scripted input + `FixedDie`):
  - `p` heals d8 + 2, lowers the count and the dragon then attacks; the player doesn't attack.
  - The heal stops at Maximum HP.
  - `p` with 0 potions, or at full HP, is refused with no turn used.
  - The combat stats show the potion count.
- **Seam 3, the player:** Maximum HP equals the rolled HP after character creation.
- **Prior art:** the full-game console tests (scripted input, captured output, timeout
  guard), the dragon tests, and the per-language key-parity test, which covers the new
  keys automatically.

## Out of Scope

- Other healing items.
- Drinking outside combat and the after-combat window (e.g. at the path choice).
- Reviving after a defeat.
- Buying several potions in one visit.
- Anything more happening after a fight. The game still ends.
- Translating the typed commands (`p`/`potion`, `y`/`n`).
- The existing Halfling Strength ≤ 0 bug (see Further Notes).

## Further Notes

- **Halfling HP ≤ 0:** fixed before this feature. Rolled HP is now at least 1, so
  Maximum HP is at least 1 too. Halfling Strength (d20 − d4) can still be 0 or below,
  which is a separate bug.
- **Balance:** about 44% of players find at least one potion (1 − 0.75²). 30 Gold is
  affordable for 90% of characters (d20 ≥ 3).
- **Domain terms:** Healing Potion and Maximum Health Points are in `GLOSSARY.md`, and the
  Merchant entry is updated with the new menu.
