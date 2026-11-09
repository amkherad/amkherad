---
name: resume-content
description: Edit Ali Kherad's resume content in this repo - the Jekyll theme data file (_data/data.yml) and the GitHub profile README. Use when adding or rewording roles, summaries, skills, education, or links, or for tailoring the resume to a job description. Not for theme layout, styling, or CI/PDF pipeline changes.
metadata:
  short-description: Edit resume content safely
---

# Resume Content

This repo is a resume, not an application. There are two content sources:

| File | What it is | Where it appears |
| --- | --- | --- |
| `_data/data.yml` | Structured resume data, rendered by the remote Jekyll theme (`sharu725/online-cv`) | The website and the generated PDF |
| `README.md` | GitHub profile banner (header image, badges, tech-stack shields, stats, contact links) | `github.com/amkherad` profile page |

Everything else is theme plumbing. Do not move content into HTML templates; `index.html` and `print.html` only include theme partials in a fixed order.

## Sections and rendering order

`index.html` and `print.html` include, in this order: career profile, education (only when `sidebar.education` is false), experiences, certifications, projects, OSS contributions, publications, recommendations, skills. A section whose data key is missing or commented out renders nothing. Adding a section means editing **both** HTML files and enabling the matching key.

Active keys in `_data/data.yml`, in file order: `sidebar`, `career-profile`, `education`, `experiences`, then a long commented block, then `skills`, then `footer`. Commented templates for `certifications`, `projects`, `oss`, and `publications` sit in that block and show the expected shape — uncomment one to enable it.

## Editing `_data/data.yml`

The file is UTF-8 without BOM, CRLF, 346 lines, with two-space indent steps starting at four spaces for top-level children. Keep it that way; a whole-file line-ending or encoding rewrite buries the real change in the diff and can break the build.

Booleans are unquoted (`about: True`, `education: False`). An empty value (`gitlab:`, `citizenship:`) is meaningful: the theme omits that contact link, so deleting the key is not equivalent to leaving it blank.

`sidebar` holds identity and contact data. Values that are handles only (`linkedin: amkherad`, `github: amkherad`); `website` and `pdf` are full URLs. `twitter` must stay quoted because it begins with `@`. `phone` is a string because of the leading `+`. `avatar` is a bare filename resolved against `assets/images/`, not a path.

`sidebar.languages.info` items use `idiom`/`level`; `sidebar.interests.info` items use `item`/`link`.

Experience entries follow this shape, and every one of the nine entries keeps it:

```yaml
experiences:
    title: Experiences
    info:
      - role: Team Lead
        time: May 2022 – Present
        company: TopTec, Iran
        details: |
          One or more prose paragraphs.

            - A bullet that supports the paragraph above.
            - Another bullet.
```

`details: |` is a literal block: line breaks are preserved. Prose sits ten spaces in, bullets twelve spaces in, and a blank line separates prose from a bullet list. Some entries open directly with the expected shape and some list bullets only; match the neighbouring entry instead of normalising the file. `time` is free text, not a date field — ranges use an en dash (–), and `Present` is literal.

`skills.title` is `Skills &amp; Proficiency` on purpose: it is an HTML entity, and it renders as `&`. Elsewhere `&` is fine unquoted (`Entity Framework`), but a value starting with a YAML indicator needs quoting.

Keep `sidebar.pdf` pointing at the artifact the pipeline actually publishes. If the PDF is renamed in `.github/workflows/jekyll.yml`, update this URL in the same change (see the `resume-build` skill).

## Editing `README.md`

The README is already UTF-8, CRLF, no BOM. Do not convert it, and do not "fix" the `·` separators — they are real U+00B7 characters that some Windows consoles display as `Â·`.

The structure is a header image, a one-line positioning statement, three badges (resume, blog, email), then About, Tech Stack, GitHub Stats, Connect. Banners and shields are remote images from `capsule-render.vercel.app`, `img.shields.io`, `github-profile-summary-cards.vercel.app` (GitHub stats and top languages) and `streak-stats.demolab.com` (streak). The official `github-readme-stats.vercel.app` instance returned 503 for every user, and the old `github-readme-streak-stats.herokuapp.com` host is unreachable, so a broken stats card is usually an upstream outage, not a malformed URL. Check the URL with `Invoke-WebRequest` before editing the README; a 200 status is not enough, because some hosts return an error SVG with 200.

The tech-stack badges and `skills.toolset` in `data.yml` only partly overlap and already differ. For example, the README lists Vue.js, Cassandra, OpenStack, Datadog, Nginx, Java and C#, none of which are in `skills.toolset`, while `skills.toolset` lists Entity Framework, which the README does not. When a skill is added or removed in one place, decide whether it belongs in the other too, and do not assume they must match.

## Editorial guardrails

This is someone's resume. Wording changes are welcome; invented facts are not. Do not add employers, titles, dates, metrics, team sizes, or technologies the user has not stated. If a bullet lacks impact, ask for the missing number rather than guessing it.

Match the existing voice: first person, direct, evidence over adjectives. Long prose lives in `career-profile.summary`; role bullets are short and specific.

Known inconsistencies — surface them, do not silently rewrite:

- `sidebar.name` and the README heading both say `Ali Kherad`, while `_config.yml` has `title: "Ali Kherad"` but `name: Ali Mousavi Kherad`. Confirm which form is preferred before changing any of them.
- TopTec and MateMachine both still read `May 2022 – Present`, but the user states TopTec began about two years after MateMachine ended, and MateMachine was first Software Architect, then CTO. Get the real start and end dates before changing either time field; do not infer them.
- AratelVC (`Sep 2019 – May 2021`) overlaps Daroopin (`Nov 2020 – May 2021`), and Sibe Sabz (`Jun 2014 – Feb 2016`) overlaps Arsin Index (`May 2014 – Jan 2015`). Verify before "fixing" the timeline.
- Mores software (`May 2021 – Sep 2022`) overlaps TopTec and MateMachine (`May 2022 – Present`) by about four months.
- The Arsin Index details say the software was built "when I was in college (bachelors)", but the BSc is dated `2016 – 2018`, after Arsin ends.
- `career-profile.summary` says `10+ years of working resume` (probably "experience"), and education says `It's was a small college`; `MateMachine` details says `I persuade them` where "persuaded" is meant.
- `sidebar.telegram` is `asdf_SHESIB`, which reads like a placeholder rather than a public handle.
- `_config.yml` sets `logo: /assets/img/avatar.jpg`, but that file does not exist; the real avatar is `assets/images/profile.png` (100×100).

## Tailoring to a job description

Start from `data.yml`, not the README, because the PDF is generated from it. Reorder or reword existing bullets to lead with what the posting asks for, and reuse only technologies the resume already claims. Keep the fact set fixed: the tailoring is emphasis, not new experience. If the posting requires something absent from both files, say so and ask whether the user has that experience instead of inserting it.

## Checking a content change

Parse the YAML before calling the edit done:

```bash
python -c "import yaml,sys; yaml.safe_load(open('_data/data.yml',encoding='utf-8')); print('ok')"
```

Then confirm you did not change the file's encoding or line endings:

```bash
git diff --stat _data/data.yml README.md
```

A diff far larger than the text you touched means an editor rewrote line endings.
