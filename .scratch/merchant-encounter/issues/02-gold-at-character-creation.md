# 02: Gold at character creation

**What to build:** During character creation, after Health Points, the player rolls for **Gold** with the same "type 'roll'" prompt pattern: d20 × 10, giving 10–200 Gold. The character stats summary shows the player's Gold. All new text ships in English, Spanish, French and Italian, accent-free (spec A2, A6; user stories 1, 2).

**Blocked by:** 01 (Replaceable dice)

**Status:** ready-for-agent

- [ ] A fourth roll prompt for Gold appears after the Health Points roll; anything other than `roll` is rejected and asked again, like the other roll prompts.
- [ ] Gold is d20 × 10 (verified with a fixed dice roller, e.g. a roll of 7 gives 70 Gold).
- [ ] The stats summary shows a Gold line with the player's current Gold.
- [ ] The new message keys exist in all four languages (the key-parity test passes).
- [ ] Existing full-game test scripts are updated for the extra roll step and still pass.
- [ ] **Gold** is defined in `GLOSSARY.md`.
