# Review der geschützten NuGet-Aktivierung

Tracking: [Issue #63](https://github.com/andregoepel/marten-configuration/issues/63). Stand: 31. Oktober 2026.
Dieser PR bleibt ein Entwurf; die Produktionsjobs auf main sind weiterhin gesperrt.

Die NuGet-Benutzeridentität andre.goepel ist bestätigt, sicher hinterlegt und durch
einen tatsächlichen geschützten OIDC-Anmeldetest ohne Upload geprüft. Die neuen
Sonar-Token wurden in beiden Produktions-Environments erfolgreich diagnostisch
geprüft; zusätzliche main-Regeln sind entfernt. Die Live-Environment-/Tag-Kontrollen
wurden erneut gelesen und passen zum reviewten Schutzmodell.

Der gehostete Main-Drill einschließlich exakter Recovery ist abgeschlossen:

| Phase | Nachweis |
| --- | --- |
| Positiver Ausgang | [Sonar 41196053666](https://github.com/andregoepel/marten-configuration/actions/runs/41196053666) |
| Gezielter Coverage-Fehler, alle 21 Tests erfolgreich | [Sonar 41197619056](https://github.com/andregoepel/marten-configuration/actions/runs/41197619056) |
| Exakte saubere Recovery, Gate OK | [Sonar 41198167945](https://github.com/andregoepel/marten-configuration/actions/runs/41198167945) |

Fehler-Coverage 33.9 % bei unveränderter 80-%-Grenze, kein Readiness-Artefakt.
Recovery-Coverage 95.1 %, neuer Nachweis an main fef9110b60a014c687d624b70cd51c8af5e4b10f gebunden,
analysisReady=true und releaseEligible=false. Quellfingerprint, Contract und Baseline
entsprechen dem sauberen Ausgang; beide Coverage-Proben sind aus main entfernt.
Testing wurde vollständig wiederhergestellt, bevor Configuration getestet wurde.

Dieser Entwurf ändert die unbedingten false-Guards von nuget-prepare.yml und
nuget-publish.yml und den LF-normalisierten Preparation-Hash in nuget-policy.json.
Alle Herkunfts-, Versions-, Commit-, Attempt-, Frische- und Handoff-Prüfungen sowie
die geschützten Environments bleiben erhalten. Ein Merge erzeugt keinen Release-Tag;
er würde künftige passende Tag-Ereignisse für den Produktionsablauf zulassen.

[Betreiberprotokoll](nuget-operator-acceptance.md) und
[sanitisierte Abnahme](../scripts/sonar/publication/production-controls/operator-acceptance-2026-10-31.json)
enthalten die Commit-/Task-/Analysis-IDs, Grenzen und noch offenen Punkte. Dazu gehören
externe Archivierung sowie
separat freizugebende echte Paket-/Symbol-Annahme und annotierte Release-Tag-Abnahme.
Keine dieser Grenzen wird durch eine erfolgreiche PR-CI oder die historischen
Readiness-Dateien aufgehoben. Vor Aktivierung Live-Kontrollen erneut lesen; nach
einem separat genehmigten Merge neue CI-/Sonar-Nachweise für den neuen exakten main
verlangen. Kein Upload, kein Release-Tag, kein Baseline-Reset und keine Token-Widerrufe.

Der autorisierte Ressourcencheck ist als unauffällige Momentaufnahme nach den Scans
abgeschlossen; Details und Grenzen stehen im Betreiberprotokoll. Ein dedizierter
Kapazitäts-/Lastspitzentest wird dadurch nicht behauptet. Der neue Notebook-SSH-Zugang
wurde mit erhaltenen bestehenden Server-Schlüsseln separat verifiziert.
