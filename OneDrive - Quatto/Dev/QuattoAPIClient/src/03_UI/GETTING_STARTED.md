# 🚀 Getting Started - Quatto API Client for SSIS

> Guia para clonar, configurar e começar a desenvolver no Quatto API Client

---

## 📋 Pré-requisitos

Antes de começar, certifique-se de ter instalado:

### Obrigatório
- ✅ **Git** (v2.40+) - [Download](https://git-scm.com/)
- ✅ **Visual Studio 2022** (Community, Professional, ou Enterprise)
  - Workload: "Integration Services" (SSIS)
  - Workload: "Data storage and processing"
- ✅ **SQL Server 2022** (Developer, Standard, ou Enterprise)
- ✅ **SQL Server Data Tools (SSDT)** v17.100+
- ✅ **.NET Framework 4.7.2** ou superior

### Recomendado
- 🎯 **Visual Studio Code** - Para editar markdown/docs
- 🎯 **SQL Server Management Studio (SSMS)** v19+
- 🎯 **Git Extensions** ou **TortoiseGit** - GUI para Git
- 🎯 **Postman** ou **Insomnia** - Para testar APIs

### Verificar Instalações

```powershell
# Verificar Git
git --version

# Verificar .NET Framework
reg query "HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\NET Framework Setup\NDP"

# Verificar Visual Studio
"${env:ProgramFiles}\Microsoft Visual Studio\2022"
```

---

## 🔧 Configuração Inicial

### 1. Clonar o Repositório

```powershell
# Clone o repositório
git clone https://github.com/ertonjm/QuattoAPIClient.git

# Entre no diretório
cd QuattoAPIClient

# Verifique o branch (deve estar em 'main' ou 'develop')
git branch -a
```

### 2. Configurar Git Localmente

```powershell
# Configure seu email e nome (se ainda não fez)
git config user.name "Seu Nome"
git config user.email "seu.email@example.com"

# Opcional: Configure como global
git config --global user.name "Seu Nome"
git config --global user.email "seu.email@example.com"

# Verifique configuração
git config --local --list
```

### 3. Abrir Solução no Visual Studio

```powershell
# Abra o arquivo .sln com Visual Studio
Start-Process ".\QuattoAPIClient.sln" -Wait
```

### 4. Restaurar NuGet Packages

No Visual Studio:
1. Tools → NuGet Package Manager → Package Manager Console
2. Execute: `Update-Package -Reinstall`

Ou via PowerShell:
```powershell
# Restaurar packages
nuget restore QuattoAPIClient.sln
```

### 5. Configurar SQL Server

```sql
-- Abra SQL Server Management Studio (SSMS)
-- Crie database para samples (opcional)
CREATE DATABASE [QuattoSamples]
GO

-- Importe o schema do sample
-- Veja: samples\01_SimpleApiConsumer\01_Setup_Database.sql
```

---

## 📖 Estrutura do Projeto

```
QuattoAPIClient/
├── 01_Source/                          # Código principal
│   ├── Helpers/                        # Classes helper (HTTP, JSON, etc)
│   ├── Models/                         # Data models
│   ├── Interfaces/                     # Contatos e interfaces
│   └── QuattoAPIClient.Source.csproj  # Projeto source
│
├── 02_ConnectionManager/               # Custom connection manager para SSIS
│   └── QuattoAPIClient.ConnectionManager.csproj
│
├── UI/                                 # Interface customizada (SSIS UI)
│   └── QuattoAPIClient.UI.csproj
│
├── samples/                            # Exemplos práticos
│   ├── 01_SimpleApiConsumer/           # Sample básico com GitHub API
│   │   ├── README.md
│   │   ├── 01_Setup_Database.sql
│   │   ├── 02_GitHub_API_Setup.md
│   │   ├── 03_SSIS_Package_Setup.md
│   │   ├── 04_Execution_Checklist.md
│   │   └── 02_Validation_Queries.sql
│   │
│   └── 02_AdvancedDataPipeline/        # Sample avançado
│       └── README.md
│
├── docs/                               # Documentação adicional
│   ├── ARCHITECTURE.md
│   ├── API_REFERENCE.md
│   └── TROUBLESHOOTING.md
│
├── .github/
│   ├── workflows/                      # CI/CD pipelines
│   │   ├── build-test.yml
│   │   └── deploy.yml
│   │
│   ├── ISSUE_TEMPLATE/                 # Templates de issues
│   │   ├── bug_report.md
│   │   ├── feature_request.md
│   │   └── config.yml
│   │
│   ├── pull_request_template.md       # Template de PR
│   └── CODEOWNERS                      # Responsáveis por áreas
│
├── .gitignore                          # Arquivos ignorados pelo Git
├── README.md                           # Documentação principal
├── LICENSE.md                          # Licença
├── CONTRIBUTING.md                     # Guia de contribuição
├── GETTING_STARTED.md                  # Este arquivo
└── QuattoAPIClient.sln                 # Solução Visual Studio

```

---

## 🏗️ Build e Testes

### Build Local

```powershell
# Via PowerShell
cd QuattoAPIClient
dotnet build --configuration Release

# Ou via Visual Studio
# Build → Build Solution (Ctrl+Shift+B)
```

### Verificar Erros

```powershell
# Ver erros de build
dotnet build --configuration Release --no-restore 2>&1 | Select-String "error"

# Build específico de projeto
dotnet build "01_Source\QuattoAPIClient.Source.csproj" -c Release
```

### Testes (quando implementados)

```powershell
# Executar testes
dotnet test --configuration Release

# Testes com cobertura
dotnet test --configuration Release /p:CollectCoverage=true
```

---

## 📚 Documentação Importante

| Documento | Descrição |
|-----------|-----------|
| [README.md](../README.md) | Overview do projeto |
| [CONTRIBUTING.md](../CONTRIBUTING.md) | Como contribuir |
| [LICENSE.md](../LICENSE.md) | Termos de licença |
| [samples/01_SimpleApiConsumer/README.md](../samples/01_SimpleApiConsumer/README.md) | Primeiro sample |
| [.github/workflows/](../.github/workflows/) | CI/CD pipelines |

---

## 🔄 Workflow Git Básico

### Criar uma Feature Branch

```powershell
# Atualize a branch principal
git checkout main
git pull origin main

# Crie uma nova branch
git checkout -b feature/minha-feature
# Ou: git switch -c feature/minha-feature

# Verifique que está na branch correta
git branch --show-current
```

### Fazer Commits

```powershell
# Veja mudanças
git status
git diff

# Stage arquivos
git add arquivo.cs
git add .  # Adicionar tudo

# Commit com mensagem clara
git commit -m "feat: adicionar novo helper para validação"
# Veja: CONTRIBUTING.md para padrão de commits

# Verifique histórico
git log --oneline -5
```

### Push e Pull Request

```powershell
# Push da branch
git push origin feature/minha-feature

# Acesse GitHub e crie uma Pull Request
# - Preencha template do PR
# - Descreva as mudanças
# - Aguarde revisão
```

### Sincronizar com Main

```powershell
# Se main foi atualizada enquanto você trabalhava
git fetch origin
git rebase origin/main

# Ou via merge (menos limpo, mas mais seguro)
git merge origin/main

# Se houver conflitos, resolva e continue
git status  # Ver conflitos
# Edite arquivos com conflitos
git add arquivo-resolvido.cs
git rebase --continue  # ou git merge --continue
```

---

## 🆘 Troubleshooting

### Erro: "SSL certificate problem"

```powershell
# Desabilitar verificação SSL (⚠️ cuidado!)
git config --global http.sslVerify false

# Ou configure certificado correto
git config --global http.sslCAInfo "C:\path\to\cert.pem"
```

### Erro: "Your local changes would be overwritten by merge"

```powershell
# Stash suas mudanças
git stash

# Pull das mudanças
git pull origin main

# Recupere suas mudanças
git stash pop
```

### Erro: "Nothing to commit, working tree clean"

```powershell
# Verifique status
git status

# Verifique if there are unstaged changes
git diff

# Se quiser resetar
git reset --hard origin/main
```

### Build Falha

```powershell
# Limpe cache de build
dotnet clean

# Restaure packages
dotnet restore

# Rebuild
dotnet build --configuration Release
```

---

## 📝 Próximos Passos

Após configurar tudo:

1. ✅ Explore o [Sample 1: SimpleApiConsumer](../samples/01_SimpleApiConsumer/README.md)
2. ✅ Leia [CONTRIBUTING.md](../CONTRIBUTING.md) para entender como contribuir
3. ✅ Configure seu IDE favorito
4. ✅ Crie sua primeira branch para uma feature/bug

---

## 💡 Dicas Úteis

### Visual Studio Extensions Recomendadas

- **GitHub Copilot** - AI-powered code completion
- **GitLens** - Git history and blame
- **NuGet Package Manager UI** - Gerenciador de packages visual
- **Resharper** (pago) - Code quality e refactoring

### PowerShell Aliases Úteis

```powershell
# Adicione ao seu $PROFILE (powershell -noprofile)
Set-Alias -Name gs -Value "git status"
Set-Alias -Name gd -Value "git diff"
Set-Alias -Name ga -Value "git add"
Set-Alias -Name gc -Value "git commit"
Set-Alias -Name gp -Value "git push"
```

### Git Config Úteis

```powershell
# Auto-correct typos
git config --global help.autocorrect 1

# Colorize output
git config --global color.ui auto

# Show branch em prompt
git config --global bash.showUpstream true
```

---

## 🤝 Precisa de Ajuda?

- 📖 Consulte [README.md](../README.md)
- 🐛 Crie uma [Issue](https://github.com/ertonjm/QuattoAPIClient/issues)
- 💬 Verifique [Discussions](https://github.com/ertonjm/QuattoAPIClient/discussions)
- 📧 Contate: support@quatto.com.br

---

**Status:** ✅ Pronto para começar!  
**Versão:** 1.0.0  
**Data:** 2025-02-20  
**Mantido por:** @ertonjm

