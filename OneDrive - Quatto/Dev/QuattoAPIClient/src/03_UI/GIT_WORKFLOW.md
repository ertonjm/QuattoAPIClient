# 🔄 Git Workflow Guide - Quatto API Client

> Padrões e convenções de Git para o projeto Quatto API Client

---

## 📊 Estratégia de Branching

Usamos **Git Flow** modificado com suporte para Continuous Delivery:

```
main (produção)
 └── release/v*.*.*
 └── develop (staging)
      ├── feature/minha-feature
      ├── bugfix/meu-bug
      ├── refactor/meu-refactor
      └── docs/minha-documentacao
```

### Tipos de Branches

| Tipo | Padrão | Descrição | De | Para |
|------|--------|-----------|----|----|
| **Main** | `main` | Produção, sempre estável | - | - |
| **Develop** | `develop` | Staging, integração | - | - |
| **Feature** | `feature/*` | Nova funcionalidade | `develop` | `develop` |
| **Bugfix** | `bugfix/*` | Correção de bug | `develop` | `develop` |
| **Hotfix** | `hotfix/*` | Emergência em produção | `main` | `main` + `develop` |
| **Refactor** | `refactor/*` | Mudança interna | `develop` | `develop` |
| **Docs** | `docs/*` | Documentação | `develop` | `develop` |
| **Release** | `release/v*.*.*` | Preparação de release | `develop` | `main` |

### Nomear Branches

```
# ✅ BOM
feature/adicionar-retry-policy
feature/melhorar-error-handling
bugfix/corrigir-timeout-infinito
bugfix/issue-123-connection-leak
refactor/simplificar-httphelper
docs/adicionar-architecture-guide
hotfix/corrigir-sql-injection

# ❌ RUIM
feature/test
feature/123
new-feature
my-fix
updating-stuff
```

---

## 📝 Commit Messages

Usamos **Conventional Commits** com escopo obrigatório:

### Formato

```
<tipo>(<escopo>): <descrição>

<corpo>

<rodapé>
```

### Exemplo

```
feat(HttpHelper): adicionar suporte a retry com exponential backoff

- Implementar CalculateBackoffDelay com 3 estratégias
- Adicionar Retry-After header handling
- Logar tentativas e latências

Closes #123
BREAKING CHANGE: RetryPolicy agora é obrigatório
```

### Tipos Permitidos

| Tipo | Descrição | Exemplo |
|------|-----------|---------|
| **feat** | Nova funcionalidade | `feat(API): adicionar autenticação OAuth` |
| **fix** | Correção de bug | `fix(HttpHelper): corrigir timeout infinito` |
| **docs** | Documentação | `docs(README): adicionar guia de setup` |
| **style** | Formatação (sem lógica) | `style(*)): aplicar .editorconfig` |
| **refactor** | Mudança interna (sem comportamento) | `refactor(Models): simplificar Response` |
| **perf** | Melhoria de performance | `perf(Cache): implementar caching` |
| **test** | Testes | `test(HttpHelper): adicionar testes de retry` |
| **ci** | CI/CD | `ci(workflows): adicionar security scan` |
| **chore** | Tarefas administrativas | `chore(deps): atualizar NuGet packages` |
| **build** | Build | `build(project): atualizar .NET target` |

### Escopos Disponíveis

```
HttpHelper, RetryPolicy, HttpResponse, LatencyStats
ConnectionManager, Models, UI
Samples, Docs, CI/CD, Dependencies
```

### Regras

- ✅ Começar com tipo + escopo
- ✅ Descrição em português, imperativo ("adicionar", não "adicionado")
- ✅ Sem ponto no final da descrição
- ✅ Primeira linha < 50 caracteres
- ✅ Corpo < 72 caracteres por linha
- ✅ Relacionar issues com "Closes #123"
- ❌ Não fazer commits com múltiplos tipos na mesma mensagem
- ❌ Não commitar code-debug ou senhas

### Exemplos Bons

```
feat(HttpHelper): adicionar suporte a HTTP compression
fix(Models): corrigir serialização de DateTime
docs(CONTRIBUTING): adicionar exemplos de git workflow
refactor(UI): remover código duplicado
test(HttpHelper): adicionar testes de rate limiting
chore(deps): atualizar Microsoft.SqlServer.Management.Sdk.Sfc
perf(Cache): implementar in-memory cache com TTL
ci(workflows): adicionar code coverage reporting
```

---

## 🔀 Fluxo de Desenvolvimento

### 1. Iniciar Feature

```powershell
# Atualize develop
git checkout develop
git pull origin develop

# Crie feature branch
git checkout -b feature/minha-feature

# Implemente
# ... código ...

# Commit
git add .
git commit -m "feat(HttpHelper): adicionar novo método"

# Push
git push -u origin feature/minha-feature
```

### 2. Abrir Pull Request

No GitHub:
1. Vá para **Pull Requests** → **New Pull Request**
2. Base: `develop` ← Compare: `feature/minha-feature`
3. Preencha template:
   - Descrição clara
   - Tipo de mudança
   - Checklist completo
   - Screenshots (se UI)
4. Clique **Create Pull Request**

### 3. Code Review

Revisores vão:
- Revisar código
- Testar localmente
- Sugerir mudanças

Seu workflow:
```powershell
# Se feedback, implemente
# ... código ...

# Commit (não force push)
git add .
git commit -m "fix: responder review sobre tratamento de erro"

# Push (sem -u, já existe)
git push origin feature/minha-feature

# GitHub auto-atualiza o PR
```

### 4. Merge

Quando aprovado:
```powershell
# Local: atualize antes de deletar
git checkout develop
git pull origin develop

# Delete branch local
git branch -d feature/minha-feature

# Delete remote (automático via GitHub)
# Ou manual:
git push origin --delete feature/minha-feature
```

---

## 🔥 Hotfix (Emergência)

Para bugs em produção:

```powershell
# Create from main
git checkout main
git pull origin main
git checkout -b hotfix/corrigir-seguranca

# Implemente fix
# ... código ...

# Commit
git commit -m "fix(Security): corrigir SQL injection em query"

# Push
git push origin hotfix/corrigir-seguranca

# No GitHub:
# 1. Create PR para main
# 2. Create PR para develop
# 3. Merge ambos quando aprovado
```

---

## 📦 Versionamento (Semantic Versioning)

Padrão: **MAJOR.MINOR.PATCH**

```
v1.2.3
 │ │ └─ PATCH: bugfix, não quebra compatibilidade
 │ └─── MINOR: nova feature, não quebra compatibilidade
 └───── MAJOR: mudança quebra compatibilidade
```

### Exemplos

```
v1.0.0   → Initial release
v1.0.1   → Patch: fix bug
v1.1.0   → Minor: nova feature
v2.0.0   → Major: breaking change
v1.0.0-beta.1  → Pre-release
v1.0.0-rc.1    → Release candidate
```

### Quando Fazer Release

1. Merge da `release/v*.*.*` para `main`
2. Tag com `git tag -a v1.2.3 -m "Release v1.2.3"`
3. Push: `git push origin v1.2.3`
4. GitHub Actions cria Release automaticamente

---

## 🏷️ Tags e Releases

### Criar Tag (Local)

```powershell
# Tag anotada (recomendado)
git tag -a v1.0.0 -m "Release version 1.0.0"

# Tag lightweight (simples)
git tag v1.0.0

# Ver tags
git tag -l
git show v1.0.0
```

### Push Tags

```powershell
# Push uma tag
git push origin v1.0.0

# Push todas as tags
git push origin --tags

# Delete tag
git tag -d v1.0.0
git push origin -d refs/tags/v1.0.0
```

### GitHub Releases

1. Vá para **Releases** → **Draft a new release**
2. Tag: `v1.0.0`
3. Target: `main`
4. Title: `Release v1.0.0`
5. Description: notas de release
6. Marque como "Pre-release" se beta
7. **Publish release**

---

## 🛡️ Branch Protection Rules

### Para `main`

- ✅ Requer PR review (1 approval)
- ✅ Requer status check (CI/CD pass)
- ✅ Dismiss stale reviews
- ✅ Require branches up to date
- ❌ Permite force push

### Para `develop`

- ✅ Requer PR review (1 approval)
- ✅ Requer status check (CI/CD pass)
- ❌ Não permite force push

---

## 🧹 Limpeza de Branches

```powershell
# Deletar branches locais merged
git branch -d feature/completa

# Deletar branch mesmo se não merged (CUIDADO!)
git branch -D feature/abandonada

# Sync local com remote (delete tracked branches)
git fetch origin --prune

# Ver branches deletadas no remote
git branch -r | grep "origin/feature/" | wc -l
```

---

## 📊 Úteis Git Commands

### Log e History

```powershell
# Log com graph visual
git log --oneline --graph --all

# Log de um arquivo
git log --oneline -- arquivo.cs

# Blame (quem fez cada linha)
git blame arquivo.cs

# Diff entre branches
git diff main develop

# Ver mudanças não committed
git diff
git diff --staged
```

### Desfazer Mudanças

```powershell
# Unstage arquivo
git restore --staged arquivo.cs

# Descartar mudanças locais (⚠️)
git restore arquivo.cs

# Desfazer último commit (keep changes)
git reset --soft HEAD~1

# Desfazer último commit (delete changes)
git reset --hard HEAD~1

# Reflog (recuperar commits perdidos)
git reflog
git checkout <commit-hash>
```

### Squash Commits

```powershell
# Interactive rebase
git rebase -i HEAD~3

# Marca commits como "squash" ou "s"
# Save e fecha editor

# Confirme
git log --oneline -5
```

---

## 🆘 Scenarios Comuns

### Scenario 1: "Cometi no branch errado"

```powershell
# Soft reset local
git reset --soft HEAD~1

# Mude de branch
git checkout feature/branch-correto

# Comite novamente
git commit -m "feat: descrição"
```

### Scenario 2: "Need to update my branch com latest main"

```powershell
git fetch origin
git rebase origin/main

# Se conflitos
# Resolva arquivos
git add .
git rebase --continue
```

### Scenario 3: "Acidentalmente deletei branch local"

```powershell
# Recupere do reflog
git reflog
git checkout -b branch-recuperada <hash>

# Ou se foi deletada no remote
git checkout -b feature/restaurada origin/feature/restaurada
```

### Scenario 4: "PR tem conflitos"

```powershell
git fetch origin
git rebase origin/develop

# Resolva conflitos em arquivos
git add .
git rebase --continue

git push -f origin feature/minha-feature
```

---

## ✅ Pre-Commit Checklist

Antes de fazer push:

- [ ] Branch name segue padrão (`feature/`, `bugfix/`, etc)
- [ ] Commits com mensagens Conventional Commits
- [ ] Sem `console.log()`, `debug()`, ou similar
- [ ] Sem secrets (tokens, passwords, keys)
- [ ] Build local passa (`dotnet build`)
- [ ] Testes passam (se houver)
- [ ] Código segue styleguide
- [ ] Documentação atualizada
- [ ] CHANGELOG atualizado (se release)

---

## 🔗 Referências

- [Git Official Docs](https://git-scm.com/doc)
- [GitHub Flow](https://guides.github.com/introduction/flow/)
- [Conventional Commits](https://www.conventionalcommits.org/)
- [Semantic Versioning](https://semver.org/)
- [Git Flow Cheatsheet](https://danielkummer.github.io/git-flow-cheatsheet/)

---

**Padrão:** Git Flow com Conventional Commits  
**Versão:** 1.0.0  
**Mantido por:** @ertonjm  

