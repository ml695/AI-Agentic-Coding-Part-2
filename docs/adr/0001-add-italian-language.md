# ADR 0001: Add Italian as a supported language

- **Status:** Accepted
- **Date:** 2026-11-09
- **Decider:** Matthew Leal
- **Commits:** `f58c5df` (Italian added), `cea0d5c` (language menu accepts names)

## Context

The game supported English, Spanish and French. All player-facing text lives in
`language_data.json`, one top-level section per language. `Messages` loads the section
whose name matches the selected language. Each section has six parts:

- `dictionary`: every message the game prints (69 keys)
- `raceMap` / `occupationMap`: words the player can type, mapped to canonical names
  (e.g. `"elfo"` → `"Elf"`)
- `displayRaceMap` / `displayOccupationMap` / `displayWeaponMap`: canonical names
  translated for display

The player chooses a language from a numbered menu in `Game.cs`. This menu runs before
any language data is loaded, so its text is hard-coded and multilingual.

> **TODO (decider):** Add the reason for choosing Italian, e.g. an assignment
> requirement, target players, or practice adding a language.

## Decision

Add Italian as a fourth language, following the same conventions as the existing three:

1. **Data, not code.** Add an `"Italian"` section to `language_data.json` with every key
   that English has. `Messages` needs no changes because it loads languages by name.
2. **ASCII only.** Spanish and French are written without accent marks ("opcion",
   "montanas"), most likely to avoid console encoding problems on Windows. Italian follows
   that convention: accents are dropped where unavoidable ("agilita") and sentences are
   reworded to avoid them elsewhere.
3. **Keep the typed commands in English.** The commands `roll`, `a`/`r` and `n`/`s`/`e`
   are hard-coded in C#, so the Italian prompts tell the player to type those exact
   commands, as the other languages do.
4. **Italian words for race and occupation input.** Players type Umano, Elfo, Nano or
   Mezzuomo for race, and Guerriero, Mago, Ladro or Arciere for occupation.
5. **Menu entry.** Add `4. Italiano` to the language menu. In a follow-up, the menu was
   changed to accept the language name as well as the number (`4`, `Italiano`, `italian`,
   `it`) and to ask again on unrecognized input. Before that change, typing `Italiano`
   silently fell back to English.
6. **Guard against missing translations.** Add the test `EveryLanguage_HasEveryEnglishKey`,
   which fails if any language is missing a key that English has. Italian-specific tests
   cover loading, the required keys and input normalization.

## Alternatives considered

- **Use `.resx` resource files (standard .NET localization).** This is more idiomatic for
  .NET, but it would mean rewriting `Messages` and changing how the whole team adds text,
  for a project where the JSON approach already works. Rejected for now.
- **Allow accented characters.** This would give more natural Italian, but it breaks
  consistency with the other languages and risks garbled text in the Windows console.
  Rejected. Revisit if the project ever sets UTF-8 console output for all languages.
- **Translate the typed commands** (e.g. `t` for *tira*). This needs per-language command
  maps in the C# code, not just new text. Rejected as out of scope.

## Consequences

**Positive**

- Players can choose Italian. Adding a language takes only a JSON section and a menu
  entry, with no change to how text is loaded.
- `EveryLanguage_HasEveryEnglishKey` now catches missing keys in *any* language, which
  protects future additions as well.

**Negative / limitations**

- **No articles before weapon names.** The text reads "attacchi il drago con spada
  lunga" where natural Italian says "con **la** spada lunga". The article depends on each
  weapon's grammatical gender, and the shared message format has no place for it.
- **Spelling without accents** ("agilita" instead of "agilità") reads slightly unnatural
  to native speakers.
- **Untranslated text remains.** The dragon's claw art `<<< claws >>>` is hard-coded in
  English in `Dragon.cs`. This affects every non-English language, not just Italian.
- **No native-speaker review.** The tests check that the text exists and that input maps
  correctly. They don't check translation quality.
- Every new message key now has to be translated into four languages instead of three.
