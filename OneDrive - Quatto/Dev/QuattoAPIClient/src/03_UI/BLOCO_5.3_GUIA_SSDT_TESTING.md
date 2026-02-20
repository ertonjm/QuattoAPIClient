# ?? BLOCO 5.3 - SSDT Wizard UI Testing

**Status**: ? PRONTO PARA TESTE  
**Tempo Estimado**: 2-3 horas  
**Tipo**: UI/Functional Testing in SSDT  

---

## ?? BLOCO 5.3 OVERVIEW

```
Objetivo: Testar wizard completo no Visual Studio SSDT
Escopo:   UI, Funcionalidade, Persistência
Ambiente: SQL Server Integration Services
Status:   ? CÓDIGO PRONTO, AGUARDANDO AMBIENTE
```

---

## ?? PRÉ-REQUISITOS

```
? Visual Studio 2019+ com SSIS instalado
? SQL Server 2019+ (qualquer edição)
? Extensão SSDT (SQL Server Data Tools)
? Nosso component registrado
? Release DLL compilada
```

---

## ?? INSTALAÇÃO DO COMPONENTE

### Passo 1: Copiar DLL

```powershell
# Copiar para pasta de componentes SSIS (SQL Server 2019)
$sourceDll = "bin\Release\ApiSourceWizard.dll"
$targetPath = "C:\Program Files\Microsoft SQL Server\160\DTS\PipelineComponents\"

Copy-Item -Path $sourceDll -Destination $targetPath -Force
```

### Passo 2: Registrar no GAC (se necessário)

```powershell
# Para SQL Server 2019 (SSIS 15.0)
C:\Windows\System32\gacutil.exe -i bin\Release\ApiSourceWizard.dll
```

### Passo 3: Registrar no Registry

```powershell
# Se necessário registrar em:
HKLM\SOFTWARE\Microsoft\Microsoft SQL Server\160\SSIS
```

---

## ?? TESTE 1: UI VISUAL

### Checklist

```
Abrir SSDT:
? Novo Integration Services Project
? Novo SSIS Package
? Data Flow Task
? Add Data Source (nosso componente)
? Direita do mouse ? Edit

Verificar UI:
? Wizard abre sem erro
? Todos os 5 tabs visíveis:
   ?? Geral
   ?? Paginação
   ?? Incremental
   ?? Armazenamento
   ?? Avançado

Visual:
? GroupBoxes com cores
   ?? Azul em General
   ?? Verde em Incremental
   ?? Laranja+Vermelho em Storage
? Controles legíveis
? Layout 900x700
? Botões OK/Cancel/Apply visíveis
```

### Evidências de Sucesso

```
Se você ver:
? Todos os tabs carregando
? Controles acessíveis
? Sem erros de rendering
? Visual profissional

? Teste 1 PASSOU ?
```

---

## ?? TESTE 2: CONTROLES FUNCIONAIS

### Teste BaseUrl

```
1. Clicar em campo "Base URL"
2. Limpar valor (Delete)
3. Sair do campo (Tab/Click outro)

Esperado:
? Campo fica vermelho (MistyRose)
? Tooltip mostra erro: "? URL obrigatória"
```

### Teste PageSize

```
1. Mudar valor para 0 (inválido)
2. Sair do campo

Esperado:
? Campo fica vermelho
? Tooltip: "? Deve estar entre 1 e 10.000"
```

### Teste WatermarkColumn

```
1. Ir para Incremental tab
2. Ativar "Ativar Incremental"
3. Limpar "Coluna Watermark"
4. Sair do campo

Esperado:
? Campo fica vermelho
? Tooltip: "? Obrigatório quando Incremental ativado"
```

### Teste Timeout

```
1. Ir para Avançado tab
2. Mudar Timeout para 5 (muito baixo)
3. Sair do campo

Esperado:
? Campo fica vermelho
? Tooltip: "? Deve estar entre 10 e 600 segundos"
```

### Teste Rate Limit

```
1. Mudar Rate Limit para 2000 (alto)
2. Sair do campo

Esperado:
? Campo fica vermelho
? Tooltip: "? Deve estar entre 1 e 10.000 rpm"
```

---

## ?? TESTE 3: VALIDAÇÃO AO SALVAR

### Cenário 1: Válido

```
1. Validar todos os campos
2. Clicar OK

Esperado:
? Validação passa
? Wizard fecha
? Component configurado
```

### Cenário 2: Com Avisos

```
1. Configurar com valores extremos:
   ?? PageSize: 8000 (alto)
   ?? Timeout: 20 (baixo)
   ?? RateLimit: 1500 (alto)
2. Clicar OK

Esperado:
? Dialog com avisos
? Message "? PageSize > 5.000 pode ser lento"
? "? Timeout < 30s pode ser curto"
? "? Taxa > 1.000 rpm pode sobrecarregar"
? Dialog: "Deseja continuar mesmo assim?"
? If Yes: Wizard fecha normalmente
? If No: Wizard permanece aberto
```

### Cenário 3: Inválido

```
1. Deixar Base URL vazia
2. Clicar OK

Esperado:
? Dialog com erros
? "Erros encontrados na configuração:"
? "• Base URL não pode estar vazia"
? OK button
? Wizard permanece aberto
```

---

## ?? TESTE 4: PERSISTÊNCIA

### Save & Reopen

```
1. Configurar todos os campos:
   ?? Base URL: https://api.test.com
   ?? Endpoint: /v2/items
   ?? Page Size: 1000
   ?? Enable Incremental: true
   ?? Watermark: updated_at
   ?? Environment: HML
   ?? Etc.

2. Clicar OK

3. Salvar Package (Ctrl+S)

4. Fechar Package

5. Reabrir Package

6. Double-click no component

Esperado:
? Wizard abre novamente
? TODOS os valores carregados corretamente
? Nenhum valor perdido
? Defaults não sobrescrevem salvos
```

---

## ?? TESTE 5: TOOLTIPS

### Verificar Tooltips

```
Passar mouse sobre cada controle por 1 segundo:

General Tab:
? Connection: "Selecione o Connection Manager..."
? Base URL: "URL base da API (ex: https://...)"
? Endpoint: "Path do endpoint (ex: /v1/orders)"
? PageSize: "Quantidade de registros (1-10000)"

Incremental Tab:
? Enable: "Carregar apenas novos registros..."
? Watermark: "Coluna para rastrear último valor"
? System: "Identificador do sistema (ex: Gladium)"
? Environment: "Ambiente (DEV/HML/PRD)"

Storage Tab:
? Mode: "Como armazenar JSON bruto"
? Target: "Caminho ou coluna de destino"
? Compress: "Remove espaços em branco"
? Hash: "Calcula SHA256 para detectar..."

Advanced Tab:
? Retries: "Número de tentativas automáticas"
? Backoff: "Estratégia de espera entre retries"
? Delay: "Delay inicial em milissegundos"
? RPM: "Limite máximo de requisições/minuto"
? Timeout: "Tempo máximo de espera em segundos"

Esperado:
? Tooltip aparece após 500ms
? Desaparece após 5 segundos ou ao sair
? Texto legível e útil
```

---

## ?? TEST SUMMARY SHEET

```
??????????????????????????????????????????????
? Teste                           ? Status   ?
??????????????????????????????????????????????
? 1. UI Visual                    ? ? TODO  ?
? 2. Controles Funcionais         ? ? TODO  ?
? 3. Validação ao Salvar          ? ? TODO  ?
? 4. Persistência                 ? ? TODO  ?
? 5. Tooltips                     ? ? TODO  ?
??????????????????????????????????????????????

Quando concluir, marcar cada teste
```

---

## ?? TEMPLATE PARA DOCUMENTAR RESULTADO

```
TESTE: [NOME]
Status: ? PASSOU / ? FALHOU / ?? AVISO

Descrição:
[O que foi testado]

Resultado Esperado:
[O que deveria acontecer]

Resultado Obtido:
[O que realmente aconteceu]

Observações:
[Notas adicionais]

Próximas ações:
[Se necessário, o que fazer]
```

---

## ?? SUCESSO CRITERIA

```
? UI abre sem erro
? Todos os 5 tabs funcionam
? Real-time validation funciona
? Tooltips aparecem
? Valores salvam e carregam
? Validação ao OK funciona
? Avisos aparecem corretamente
? GroupBoxes com cores corretas
? Sem exceptions
? Código compilado release funciona
```

---

## ? PRÓXIMO PASSO

Quando concluir todos os testes:

1. Documentar resultados
2. Se todos ? PASSOU: Ir para BLOCO 5.4
3. Se algum ? FALHOU: Debugar e corrigir
4. Se ?? AVISO: Documentar e decidir

---

**Versão**: 3.0.3  
**Status**: ?? BLOCO 5.3 PRONTO PARA TESTE  
**Próximo**: BLOCO 5.4 (Connection Manager Integration)

