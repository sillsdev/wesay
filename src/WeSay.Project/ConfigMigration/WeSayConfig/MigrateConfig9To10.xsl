<?xml version="1.0" encoding="UTF-8" ?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<!-- don't do anything to other versions -->
	<xsl:template match="configuration[@version='9']">
		<configuration version="9">
			<xsl:apply-templates mode ="Migrate9To10"/>
		</configuration>
	</xsl:template>

	<xsl:template match="@*|node()" mode="Migrate9To10">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"  mode="Migrate9To10"/>
		</xsl:copy>
	</xsl:template>

	<xsl:template match="@*|node()" mode="identity">
		<xsl:copy>
			<xsl:apply-templates select="@*|node()"  mode="identity"/>
		</xsl:copy>
	</xsl:template>

	<!-- ============= actual changing starts here ================ -->
	<xsl:template match="task[@taskName='Dashboard']" mode="Migrate9To10">
		<task taskName="Dashboard">
			<xsl:attribute name="visible">
				<xsl:value-of select="current()/@visible"/>
			</xsl:attribute>
			<touchAllEntriesCompleted>False</touchAllEntriesCompleted>
		</task>
	</xsl:template>
</xsl:stylesheet>