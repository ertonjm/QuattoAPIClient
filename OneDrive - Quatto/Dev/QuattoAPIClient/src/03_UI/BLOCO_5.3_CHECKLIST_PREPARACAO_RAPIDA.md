# ⚡ CHECKLIST DE PREPARAÇÃO - BLOCO 5.3

**Antes de começar os testes - 5 minutos!**

---

## 🔍 VERIFICAÇÃO DO AMBIENTE

### Passo 1: Visual Studio

```
[x] Visual Studio está aberto?
    └─ Se não: Abrir Visual Studio

[x] SSDT (SQL Server Data Tools) está instalado?
    └─ Verificar: Tools → Extensions and Updates → Search "SSDT"
    └─ Se não: Instalar via VS Installer

[x] SQL Server Integration Services está instalado?
    └─ Verificar: Start → SQL Server Management Studio
    └─ Se não: Instalar SQL Server with IS feature
```

### Passo 2: Componente

```
[ ] Release build foi compilada recentemente?
    └─ Build → Rebuild Solution → Aguardar "Build completed"
    
[ ] DLL está em PATH correto?
    └─ Local: C:\Program Files (x86)\Microsoft SQL Server\...\
    └─ Ou onde SSDT procura componentes SSIS

[ ] Componente está registrado?
    └─ CMD (Admin): 
       cd C:\caminho\da\dll\
       regasm.exe QuattoApiClient.dll /codebase
       
[ ] SSDT foi reaberto após registrar?
    └─ Importante: Fechar e reabrir Visual Studio
```

### Passo 3: Package de Teste

```
[ ] Você tem um Integration Services Package pronto?
    └─ File → New Project → Integration Services Project
    └─ Add Data Flow Task
    └─ Salvar como: TestPackage.dtsx

[ ] Package está no local acessível?
    └─ C:\Projetos\SSIS\TestPackage.dtsx
    └─ Ou similar
```

### Passo 4: .NET Framework

```
[ ] .NET Framework 4.7.2 ou superior está instalado?
    └─ Start → Settings → Apps → Programs & Features
    └─ Procurar: .NET Framework 4.7.2 or higher
    └─ Se não: Download from Microsoft.com
```

---

## ✅ CHECKLIST PRÉ-TESTE

```
AMBIENTE:
[ ] Visual Studio aberto
[ ] SSDT instalado e ativo
[ ] SQL Server IS instalado
[ ] .NET 4.7.2 instalado

COMPONENTE:
[ ] Release build compilada
[ ] DLL registrada (regasm.exe)
[ ] Visual Studio foi reaberto após regasm
[ ] Caminho da DLL confirmado

PACKAGE:
[ ] Package criado e salvo
[ ] Data Flow Task adicionada
[ ] Package acessível no disco

DOCUMENTAÇÃO:
[ ] BLOCO_5.3_TEST_RESULTS_TEMPLATE.md pronto
[ ] BLOCO_5.3_ISSUES_LOG_TEMPLATE.md pronto
[ ] BLOCO_5.3_EVIDENCE_SCREENSHOTS_TEMPLATE.md pronto
[ ] BLOCO_5.3_TUTORIA_PRATICA_COM_SCREENSHOTS.md lido

TUDO CHECADO?
[ ] SIM → Ir para PASSO 1
[ ] NÃO → Resolver acima antes
```

---

## 🚀 COMEÇAR OS TESTES

### Se tudo está OK:

```
1. Abrir Visual Studio
2. Abrir seu TestPackage.dtsx
3. Data Flow Task → Toolbox → Procurar por "Quatto"
4. Arrastar nosso componente para data flow
5. Clicar direita → Edit
6. Wizard abre → TESTE 1 COMEÇA!

Tempo estimado: 2-3 horas
Status: PRONTO PARA COMEÇAR ✅
```

---

## 🆘 SE ALGO ERRAR DURANTE OS TESTES

### Erro: "Component not found in toolbox"

```
SOLUÇÃO:
1. Fechar Visual Studio completamente
2. CMD (Admin):
   cd C:\caminho\da\DLL\
   regasm.exe QuattoApiClient.dll /codebase
   regasm.exe QuattoApiClient.dll /u (para unregister, se needed)
   regasm.exe QuattoApiClient.dll /codebase (registrar novamente)
3. Reabrir Visual Studio
4. Procurar novamente na Toolbox
```

### Erro: "Exception when opening wizard"

```
SOLUÇÃO:
1. Verificar que DLL está no lugar certo
2. Recompilar release build
3. Registrar novamente (regasm)
4. Fechar Visual Studio
5. Tentar novamente
```

### Erro: "Package won't save"

```
SOLUÇÃO:
1. Fechar o wizard (Cancel)
2. Tentar salvar package (Ctrl+S)
3. Se não salva: Package está read-only?
   → Propriedades do arquivo → Uncheck "Read-only"
4. Tentar novamente
```

---

## 📋 DOCUMENTOS QUE VOCÊ VAI USAR

```
Durante os testes:

1. BLOCO_5.3_TUTORIA_PRATICA_COM_SCREENSHOTS.md
   └─ Siga os passos (você está aqui agora!)

2. BLOCO_5.3_TEST_RESULTS_TEMPLATE.md
   └─ Preencha após cada teste

3. BLOCO_5.3_EVIDENCE_SCREENSHOTS_TEMPLATE.md
   └─ Tire screenshots ou faça anotações

4. BLOCO_5.3_ISSUES_LOG_TEMPLATE.md
   └─ Se encontrar problemas

5. BLOCO_5.3_FINAL_SIGN_OFF_TEMPLATE.md
   └─ Preencha ao final de todos os 5 testes
```

---

## ⏱️ CRONOGRAMA ESTIMADO

```
Pré-teste (agora):          5 min
Setup Visual Studio:        5 min
TESTE 1 (UI Visual):        15 min
TESTE 2 (Controles):        20 min
TESTE 3 (Validação):        20 min
TESTE 4 (Persistência):     20 min
TESTE 5 (Real-time Val):    15 min
Documentar resultados:      30 min
────────────────────────────────
TOTAL:                      2h 20min (~ 2.5h)
```

---

## 🎯 PRÓXIMOS PASSOS APÓS BLOCO 5.3

```
1. Preencher todos os templates
2. Revisar resultados
3. Decidir se prossegue para BLOCO 5.4
4. Se bloqueadores: registrar em ISSUES_LOG
5. Se OK: Prosseguir para BLOCO 5.4 (Connection Manager)
```

---

**PRONTO PARA COMEÇAR? 🚀**

→ Vá para: BLOCO_5.3_TUTORIA_PRATICA_COM_SCREENSHOTS.md

→ Siga os passos de TESTE 1 até TESTE 5

→ Boa sorte! ✅

