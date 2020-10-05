import React, { useState } from 'react';
import PropTypes from 'prop-types';

const PageNavigation = ({
	pagination: { currentPage, itemsPerPage, totalItems, totalPages },
	pageNumber,
	setPageNumber
}) => {
	const numPages = [];
	for (let i = 1; i <= totalPages; i++) {
		numPages.push(i);
	}

	// console.log(currentPage);
	// console.log(totalPages);
	// if (currentPage === totalPages) {
	// 	isLastPage(true);
	// }

	return (
		<div className='pagination'>
			<ul>
				<li onClick={() => setPageNumber(1)}>
					<i className='fas fa-angle-double-left' />
				</li>
				<li onClick={() => pageNumber !== 1 && setPageNumber(pageNumber - 1)}>
					<i className='fas fa-angle-left' />
				</li>
				{numPages.splice(currentPage - 1, 5).map(
					(value, key) =>
						key === 0 ? (
							<li className='active' key={key}>
								{value}
							</li>
						) : (
							<li onClick={() => setPageNumber(value)} key={key}>
								{value}
							</li>
						)
				)}
				<li onClick={() => pageNumber !== totalPages && setPageNumber(pageNumber + 1)}>
					<i className='fas fa-angle-right' />
				</li>
				<li onClick={() => setPageNumber(totalPages)}>
					<i className='fas fa-angle-double-right' />
				</li>
			</ul>
		</div>
	);
};

export default PageNavigation;
