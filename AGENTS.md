# Repository guide

This repository is Ali Kherad's resume: a Jekyll site on the remote `sharu725/online-cv` theme, published to GitHub Pages, with the resume rendered to PDF in CI.

Content lives in `_data/data.yml` (structured resume data, drives the site and the PDF) and `README.md` (the GitHub profile banner). Everything else is theme plumbing: `_config.yml`, `index.html`, `print.html`, `_sass/custom.scss`, `scripts/inline-html.sh`, and `.github/workflows/jekyll.yml`.

Read the matching skill before editing:

- `.agents/skills/resume-content/SKILL.md` — roles, summary, skills, education, links, README badges, and tailoring to a job description.
- `.agents/skills/resume-build/SKILL.md` — config, layouts, Sass, the PDF pipeline, local preview, and deploy failures.

Two constraints that are easy to break: keep `_data/data.yml` as UTF-8 without BOM with CRLF line endings, and keep `baseurl` (`/amkherad`), the `sed` prefix in the workflow, the PDF filename, and `sidebar.pdf` consistent with each other.

Do not invent experience, dates, or metrics. Flag inconsistencies in the timeline or wording instead of silently correcting them.
