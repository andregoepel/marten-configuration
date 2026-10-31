# Betreiberabnahme vor NuGet-Aktivierung

Tracking: [Issue #63](https://github.com/andregoepel/marten-configuration/issues/63). Stand: 31. Oktober 2026.
Aktueller sauberer main nach Recovery: fef9110b60a014c687d624b70cd51c8af5e4b10f.

## Abgeschlossen

- Besitzer und effektive NuGet-Identität andre.goepel bestätigt. Beide bestehenden
  NUGET_USER-Secrets wurden mit ausdrücklicher Zustimmung gesetzt. Der tatsächliche
  [geschützte OIDC-Anmeldetest](https://github.com/andregoepel/marten-configuration/actions/runs/41111071922) war in Original-Attempt 1
  erfolgreich; echter Publisher skipped, keine Artefakte, kein Upload. Der kurzlebige
  Schlüssel wurde nicht archiviert. Die NuGet-Policy bindet das exakte vorhandene Paket
  an Repository, nuget-publish.yml und Environment nuget-publish, nur neue Versionen.
- Produktions-Environments und unveränderliche Tag-Regeln am 31. Oktober erneut
  gelesen: Preparation nur v*, Publisher nur main, Reviewer 22834588, Selbstfreigabe
  erlaubt und Admin-Bypass aus. Keine temporäre main-Regel vorhanden. Das Publisher-
  Environment überschreibt NUGET_USER nicht. Keine bestehenden Tokens widerrufen.
- Geschützter Main-Coverage-Fehler und exakte Recovery vollständig durchgeführt.
  Testing wurde einschließlich Recovery beendet, bevor Configuration begann.
  Alle 21 erforderlichen Tests blieben auch auf der Fehlerrevision erfolgreich.

| Phase | Commit | Sonar-Lauf | Gate / Gesamt-Coverage |
| --- | --- | --- | --- |
| Sauberer Ausgang | 33a47849ceea4426b3ab838961d7a948dc42a53e | [41196053666](https://github.com/andregoepel/marten-configuration/actions/runs/41196053666) | OK / 95.1 % |
| Coverage-Probe | 59c79e9f454a553e1ded6c3393ac82515542bb97 | [41197619056](https://github.com/andregoepel/marten-configuration/actions/runs/41197619056) | ERROR / 33.9 % |
| Exakte Recovery | fef9110b60a014c687d624b70cd51c8af5e4b10f | [41198167945](https://github.com/andregoepel/marten-configuration/actions/runs/41198167945) | OK / 95.1 % |

Jede Sonar-Ausführung war ein neuer Original-Attempt 1. Die Ausgangs-CI lief wegen
abgelaufener Vortagsevidenz in frischem Attempt 3; Fehler- und Recovery-CI jeweils
in Attempt 1. Alle vorgeschriebenen Restore-, Format-, Build-, Test- und Audit-
Schritte wurden vollständig als erfolgreich verifiziert.

Der negative CE-Task wurde erfolgreich verarbeitet. Das unveränderte coverage < 80-
Gate lehnte konkret die importierte unbedeckte Produktionsprobe ab. Es gab für die
Fehlerrevision nur die Analyse-Diagnose und **kein** positives Readiness-Artefakt.
Ein beliebiger Build-/Netzwerkfehler wurde nicht als Negativnachweis akzeptiert.

[Fehler-PR #75](https://github.com/andregoepel/marten-configuration/pull/75) und
[Recovery-PR #78](https://github.com/andregoepel/marten-configuration/pull/78) wurden ausdrücklich freigegeben.
Die tatsächliche Rücknahme wurde auf dem gemergten Fehler-main aufgebaut und ihre
Probe vor Entfernung gegen den reviewten Blob geprüft. Der gesamte bereinigte Git-
Quellbaum und der neue Analyse-Quellfingerprint entsprechen exakt dem sauberen
Ausgangsstand. Contract, spezifische Baseline, Gate und eingefrorene Profile blieben
unverändert. Die neue Recovery hat eigene Task-/Analysis-IDs und einen frischen
Readiness-Nachweis: analysisReady=true, releaseEligible=false.

Die sanitisierte Aufzeichnung mit IDs, Fingerprints, Tests und Ablaufzeiten liegt in
[operator-acceptance-2026-10-31.json](../scripts/sonar/publication/production-controls/operator-acceptance-2026-10-31.json).
Sie ist ein historischer Abnahmebeleg und autorisiert keinen späteren Release.
Die früheren API-Status-/Schrittinventar-Fehler bleiben im Laufverlauf erhalten;
der geprüfte gemeinsame Verifier-Pin und vollständige CI-Schrittnachweise wurden
vor den erfolgreichen Läufen verlangt. Keine Prüfanforderung wurde entfernt.

## Grenzen und verbleibende Punkte

- Servergesundheit GREEN und leere CE-Warteschlange wurden vor jeder sequenziellen
  Scan-Freigabe geprüft. Autorisierte Host-Stichproben nach den Scans: mindestens
  6.708 MiB von 11.914 MiB RAM verfügbar (56,3 %), 175.789 MiB Systemplatte frei
  (74,2 %), einminütige Last 0,12–0,17. Sonar belegte 2,277 GiB seines 5-GiB-Limits
  (45,55 %), seine Datenbank 124,2 MiB ihres 1-GiB-Limits (12,13 %). Dies sind
  Momentaufnahmen nach den Scans, kein Lastspitzen-, Langzeit- oder Kapazitätsdrill.
  Der Host-Fingerabdruck wurde unabhängig bestätigt. Der ausdrücklich beauftragte
  Notebook-Public-Key wurde mit Backup und Erhalt bestehender Schlüssel ergänzt;
  Anmeldung nur mit dem lokalen Notebook-Privatkey, ohne Agent/temporären Zugang,
  wurde separat verifiziert. Keine globale SSH-Konfiguration geändert, keine privaten
  Schlüsselinhalte ausgegeben oder auf den Server übertragen. Außer der genehmigten
  authorized_keys-Ergänzung wurde keine Server-Konfiguration geändert.
- Die vier Produktions-Token-Diagnosen belegen Identität und private Projekt-Lesegrenzen;
  der Main-Drill verwendet die bestehenden sonar-main-Credentials. Er ist kein neuer
  Analyse-Submit-Nachweis jedes zusätzlichen Produktions-Tokens.
- Tatsächlicher Paket-/Symbol-Upload und entfernte Annahme sowie gehostetes Verhalten
  annotierter Release-Tags wurden unter dem Veröffentlichungsverbot nicht getestet.
  Der rote Main-Drill beweist die Readiness-Ablehnung, keinen realen NuGet-Uploadversuch.
- Externe Archivierung bleibt unkonfiguriert. Sanitisierte Belege liegen im Review und
  in den jeweiligen Actions-Artefakten. Keine Token-Werte wurden aufgenommen.

## Aktivierungsreview

Auf main sind beide Produktionsjobs weiterhin unbedingt gesperrt. Der getrennte
[Aktivierungsentwurf #73](https://github.com/andregoepel/marten-configuration/pull/73) entfernt nur diese Sperren
und aktualisiert den gebundenen Preparation-Hash. Er bleibt ungemergt. Vor späterem
Release müssen die noch offenen Punkte bewertet, ein Aktivierungs-Merge separat
freigegeben und neue erfolgreiche CI-/Sonar-Nachweise für genau dessen main-Commit
verlangt werden. Version, Commit und Release-Tag benötigen eine eigene Freigabe.
Keine Pakete veröffentlicht, keine Release-Tags angelegt; Tracking-Issue bleibt offen.
