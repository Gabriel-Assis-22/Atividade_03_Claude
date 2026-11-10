#import "helpers.typ": *

= Quadro de entregas

A tabela a seguir consolida automaticamente os prazos planejados e os registros temporais reais de entrega (data e hora no fuso `America/Sao_Paulo`, UTC-3) apurados nas fichas individuais da Seção 5:

#context {
  let l = registro.final().sorted(key: a => a.numero)
  table(
    columns: (auto, 1fr, 2.3cm, 3.3cm, 2.8cm),
    align: (center, left, center, center, center),
    fill: (x, y) => if y == 0 { luma(235) } else { none },
    table.header([*Nº*], [*Atividade*], [*Data \ planejada*], [*Data \ realizada*], [*Situação*]),
    ..l.map(a => ([#a.numero], [#a.titulo], [#a.planejada], [#a.realizada], [#a.situacao])).flatten(),
  )
}

